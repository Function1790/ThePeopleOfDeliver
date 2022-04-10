import socket as sk
import threading as th
import json

HOST = "127.0.0.1" 
PORT = 1089

client = sk.socket(sk.AF_INET, sk.SOCK_STREAM)
client.connect((HOST, PORT))

def Log(title, content):
    print(f"[{title}] >> {content}")

def Receive_Handle():
    while True:
        try:
            data = client.recv(1024)
            text = data.decode("utf-8")
            if text!="":
                Log("Receive", text)
        except:
            Log("Except","Error")
            break

def Send(text):
    Log("Send", text)
    client.sendall(text.encode("utf-8"))

def Command_Dump(_cmd, _arg):
    return {"Command":_cmd, "Arg":_arg}

receive=th.Thread(target=Receive_Handle)
receive.start()
while True:
    print(end=">>")
    text=input()
    if text=="exit":
        break
    elif text=="login":
        _send=Command_Dump("login", {"uid":"test123","upw":"test123"})
        Send(json.dumps(_send))
    elif text=="logout":
        _send=Command_Dump("logout", "")
        Send(json.dumps(_send))
client.close();