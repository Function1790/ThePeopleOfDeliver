import socket as sk
import threading as td
import UserDB as user
import DeliverDB as deliver
import random as r
import json

#Constant
HOST = "127.0.0.1" 
PORT = 1089

#Variable
#클라이언트를 담는 리스트
user_list=[]
#접속 ID 생성 부호
thex=["0","1","2","3","4","5","6","7","8",
    "9","a","b","c","d","e","f","g","h","i",
    "j","k","l","m","n","o","p","q","r","s",
    "t","u","v","w","x","y","z"]

#클라이언트 접속 ID 생성
def Create_UID():
    result=""
    for i in range(10):
        result+=str(thex[r.randint(0,len(thex)-1)])
    return result

#로그
def Log(title, content):
    print(f"[{title}] >> {content}")

#C#에서의 json형태로 변환
#아직 미완성
def CS_dump(obj):
    return json.dumps(obj)

#커멘드 형태로 Dump
def Command_Dump(_cmd, _arg):
    return {"Command":_cmd, "Arg":_arg}

#Deliver Table에 주문 추가
def Deliver_Insert(text, uid):
    _args=text["Arg"]
    send_data=Command_Dump("deliver_insert", "false")
    print(_args["market_name"], _args["menu"], uid)
    if deliver.Insert_Deliver(_args["market_name"], _args["menu"], uid):
        send_data["Arg"]="true"
    return CS_dump(send_data)

#클라이언트 접속시 객체로 전환
class User:
    def __init__(self, client):
        self.uid=Create_UID()
        Log("Join",f"User<{self.uid}>")
        self.client=client
        self.thread_receive=td.Thread(target=self.Receive_Handle)
        self.thread_receive.daemon=True
        self.thread_receive.start()
        self.uid=None
        self.isLogined=False

    #로그인 요청시 DB에서 정보확인
    def Login(self, text):
        _args=text["Arg"]
        send_data=Command_Dump("login", "false")
        if user.Is_Exist(_args["uid"], _args["upw"]):
            send_data["Arg"]="true"
            self.uid=_args["uid"]
            self.isLogined=False
        return CS_dump(send_data)

    #클라이언트에서 받은 데이터 처리
    def Receive_Handle(self):
        while True:
            try:
                data = self.client.recv(1024)
                print(data.decode("utf-8"))
                text = json.loads(data.decode("utf-8"))
                Log("Get",text)
                #명령어 처리
                _command=text["Command"]
                if _command=="login":#로그인
                    self.Send(self.Login(text))
                elif _command=="logout":#로그아웃
                    self.uid=None
                    self.isLogined=False
                    self.Send(CS_dump(Command_Dump("logout", "")))
                elif _command=="deliver_insert":#주문 추가
                    self.Send(Deliver_Insert(text, self.uid))
                elif _command=="deliver_complete":
                    pass             
            except:
                break
        Log("Exit",f"User<{self.uid}>")
        self.client.close()

    #클라언트로 데이터 전달
    def Send(self, text):
        if text=="":
            return False
        Log("Send",f"User<{self.uid}> {text}")
        self.client.sendall(text.encode("utf-8"))

#모든 클라이언트에 데이터 전달
def Alert(user:User, text:str):
    for i in user_list:
        if i!=user:
            i.Send(f"User<Alert, {user.uid}> : {text}")
    return True

#Main
main_server=sk.socket(sk.AF_INET, sk.SOCK_STREAM)
Log("Start", f"PORT : {PORT}, HOST : {HOST}")

main_server.setsockopt(sk.SOL_SOCKET, sk.SO_REUSEADDR, 1)
main_server.bind((HOST, PORT))
main_server.listen(100)

while True:
    n=len(user_list)
    client, addr = main_server.accept()
    user_list.append(User(client))