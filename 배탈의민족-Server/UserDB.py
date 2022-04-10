import pymysql as mysql
import random as r

def Log(title, content):
    print(f"[{title}] >> {content}")

#insert into user (uid, upw) value("test123", "test123");

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

#유저정보 가져오기
def Get_Columns():
    return Command(f'select * from user;')

#유저 ID 와 PW 확인
def Is_Exist(uid, upw):
    sql_result=Command(f"select * from user where uid='{uid}' and upw='{upw}';")
    if sql_result!=():
        return True
    return False

#회원가입
def Insert_User(uid, upw):
    if not Is_Exist(uid, upw):
        Command(f"insert into user (uid, upw) value('{uid}', '{upw}');")
        return True
    return False