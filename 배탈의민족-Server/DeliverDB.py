import pymysql as mysql
import random as r

def Log(title, content):
    print(f"[{title}] >> {content}")

#insert into deliver (market_name, menu, receiver_uid) values("Pizza", "{'불고기 피자':'false'}", "test123");

db = mysql.connect(
    user='root', 
    passwd='', 
    host='127.0.0.1', 
    db='coding', 
    charset='utf8'
)

cursor = db.cursor(mysql.cursors.DictCursor)

#Sql
def Command(sql:str):
    try:
        cursor.execute(sql)
        db.commit()
        return cursor.fetchall()
    except mysql.err.ProgrammingError:
        return ()

#select * from table
def Get_Columns():
    return Command(f'select * from user;')

#유저 존재 확인
def Is_Exist_User(uid):
    sql_result=Command(f"select * from user where uid='{uid}'")
    if sql_result!=():
        return True
    return False

#배달 미도착 여부
def is_Delivering(order_num):
    if Command(f"select * from deliver where order_num={order_num}")==():
        return False
    return True

#가게 이름으로 남은 배달주문 확인
def Deliver_List(market_name):
    result=Command(f"select * from deliver where market_name={market_name}")
    if result!=():
        return result
    return None

#주문추가
def Insert_Deliver(market_name, menu ,receiver_uid):
    if Is_Exist_User(receiver_uid):
        Command(f"insert into deliver (market_name, menu, receiver_uid) values(\"{market_name}\", \"{menu}\", \"{receiver_uid}\");")
        return True
    return False

#주문완료
def Complete_Devlier(order_num:int):
    Command(f"delete from deliver where order_num={order_num};")
    if is_Delivering(order_num)==True:
        return False
    return True