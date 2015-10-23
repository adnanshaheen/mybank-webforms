using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ChangePwd : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (SessionFacade.USERNAME == null)
        {
            SessionFacade.PAGEREQUESTED = Request.ServerVariables["SCRIPT_NAME"];
            Response.Redirect("Login.aspx");
        }
    }
    protected void btnChangePwd_Click(object sender, EventArgs e)
    {

    }
}