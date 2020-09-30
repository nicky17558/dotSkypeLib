using SkypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotSkypeLibSample
{
    public partial class sample : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DotSkype dotSkype = new DotSkype();
            var loginToken = dotSkype.SendSoapLogin("xxxxxx", "password");
            var exchangeToken = dotSkype.ExchangeSkypeToken(loginToken);
            var userName = dotSkype.GetSkypeUserProfile(exchangeToken);
            var contact = dotSkype.GetSkypeUserContactInfoList(userName, exchangeToken);
            dotSkype.QueryThread(exchangeToken, "");
            dotSkype.SendMultipleText(exchangeToken, "test", new List<string>() { "sendMessageUserId" });
        }
    }
}