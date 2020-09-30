# dotSkypeLib
dot net skype Api lib


ㄏノよ猭

    DotSkype dotSkype = new DotSkype();

    //祅恶SKYPE眀腹盞絏
    var loginToken = dotSkype.SendSoapLogin("xxxxxx", "password");

    //ユ传SKYPETOKEN
    var exchangeToken = dotSkype.ExchangeSkypeToken(loginToken);

    //琩高UserId
    var userName = dotSkype.GetSkypeUserProfile(exchangeToken);

    //琩高狟ね睲虫
    var contact = dotSkype.GetSkypeUserContactInfoList(userName, exchangeToken);

    //琩高┮ΤConversaction
    //dotSkype.QueryThread(exchangeToken, "");

    //癳秖絋玂场獺
    //dotSkype.SendMultipleText(exchangeToken, "test", new List<string>() { "sendMessageUserId" });

    //癳虫獺
    //dotSkype.SendText(exchangeToken, "test","sendMessageUserId");
