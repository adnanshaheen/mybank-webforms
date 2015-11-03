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
        IBusinessAbstraction iba = GenericFactory<BusinessAbstraction, IBusinessAbstraction>.CreateInstance();

        try
        {
            string oldPwd = Utils.StripPunctuation(txtOldPwd.Text);
            string newPwd = Utils.StripPunctuation(txtNewPwd.Text);
            string rePwd = Utils.StripPunctuation(txtRetypePwd.Text);
            if (rePwd.Equals(newPwd))
            {
                string userName = Utils.StripPunctuation(SessionFacade.USERNAME);
                string chkAcctNum = iba.IsValidUser(userName, oldPwd);
                if (chkAcctNum != "")
                {
                    if (iba.ChangePassword(userName, oldPwd, newPwd))
                        lblStatus.Text = "Password updated successfully!";
                    else
                        lblStatus.Text = "Couldn't change password!!!";
                }
                else
                    lblStatus.Text = "Invalid old password ...";
            }
            else
                lblStatus.Text = "Password mismatch ...";
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
}