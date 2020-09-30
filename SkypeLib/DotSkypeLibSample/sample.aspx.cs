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
            var loginToken = dotSkype.SendSoapLogin("", "");
            var exchangeToken = dotSkype.ExchangeSkypeToken(loginToken);
            var userName = dotSkype.GetSkypeUserProfile(exchangeToken);
            var contact = dotSkype.GetSkypeUserContactInfoList(userName, exchangeToken);

            var regToken =dotSkype.GetRegisterToken(exchangeToken);
            dotSkype.SendText(regToken, "Test", "");
            //dotSkype.QueryThread(exchangeToken, "");
            //dotSkype.SendMultipleText(exchangeToken, "test", new List<string>() { "sendMessageUserId" });
        }
    }
}