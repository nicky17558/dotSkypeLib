# dotSkypeLib
dot net skype Api lib


使用方法

    DotSkype dotSkype = new DotSkype();

    //登入填SKYPE帳號密碼
    var loginToken = dotSkype.SendSoapLogin("xxxxxx", "password");

    //交換SKYPETOKEN
    var exchangeToken = dotSkype.ExchangeSkypeToken(loginToken);
    
    //交換發話用token
    var regToken =dotSkype.GetRegisterToken();

    //查詢自己的UserId
    var userName = dotSkype.GetSkypeUserProfile();

    //查詢自己的朋友清單
    var contact = dotSkype.GetSkypeUserContactInfoList(userName);

    //查詢自己的所有Conversaction
    //dotSkype.QueryThread();

    //送大量確保全部信息
    //dotSkype.SendMultipleText(regToken, "test", new List<string>() { "sendMessageUserId" });

    //送單封信息
    //dotSkype.SendText(regToken, "test","sendMessageUserId");
