# Server
핫소스통 서버입니다.

## 요청과 응답(ing)
[내 블로그](https://blog-for-sw-study.tistory.com/81)

## API서버의 요청과 응답 구조체
현재는 [이 곳](https://github.com/HotSauceTong/Server/tree/create_project_and_regist%2Bdatabase/GameAPIServer/GameAPIServer/ReqResModels). 


## ErrorCode
```C#
public enum ErrorCode : Int16
{
    None = 0,
    // Db Error 50 ~ 99
    SessionError = 50,
    GameDbError = 51,

    // request format Error 100 ~ 199
    InvalidRequestFormat = 100,
    InvalidEmailFormat = 101,
    InvalidPasswordFormat = 102,
    InvalidClientVersion = 103,
    InvalidNicknameFormat = 104,

    // Regist Error
    EmailAlreadyExist = 201,
    NicknameAlreadyExist = 202,

}
```
