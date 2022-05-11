using System;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Security.Principal;
using System.Collections.Specialized;
using ExtensionMethods;

/// <summary>
///   檢查Session是否過期，重新取得登入資訊
/// </summary>
public class SecurityCheck : System.Web.UI.Page
{
    protected override void OnLoad(System.EventArgs e)
    {
        try
        {
            //[檢查參數] Session是否已過期
            if (HttpContext.Current.Session["Login_UserID"] == null)
            {
                //清除Session
                Session.Clear();

                //判斷是否使用內部AD驗證
                string IsAD = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_IsUse"];
                if (IsAD.ToUpper().Equals("Y"))
                {
                    if ((Request.Cookies["PK_UserSID"] == null))
                    {
                        //AD登入驗證 - 網域電腦登入後自動驗證
                        CheckAD_Auto();
                    }
                    else
                    {
                        //AD登入驗證 - 手動輸入帳密
                        CheckAD_Input(Request.Cookies["PK_UserSID"].Value.ToString());
                    }

                    base.OnLoad(e);
                }
                else
                {
                    //導向登入頁
                    Response.Redirect("{0}ServiceLogin".FormatThis(Application["WebUrl"]));

                }

            }
            else
            {
                base.OnLoad(e);
            }
        }
        catch (Exception)
        {
            throw;
        }

    }

    /// <summary>
    /// AD登入驗證 - 網域電腦登入後自動驗證
    /// </summary>
    private void CheckAD_Auto()
    {
        //取得登入相關資訊
        IPrincipal userPrincipal = HttpContext.Current.User;
        WindowsIdentity windowsId = userPrincipal.Identity as WindowsIdentity;
        if (windowsId == null)
        {
            //找不到此SID, 導向登入錯誤頁, (請先登入網域)
            Response.Redirect(ErrPage("1001"));

            return;
        }
        else
        {
            SecurityIdentifier sid = windowsId.User;
            //取得屬性值(Sid / DisplayName / AccountName / Guid / 帳戶類型)
            StringCollection listAttr = ADService.getAttributesFromSID(sid.Value);
            if (listAttr == null)
            {
                //找不到此SID, 導向登入錯誤頁, (帳號未建立或未登入網域)
                Response.Redirect(ErrPage("1002"));
                return;
            }
            else
            {
                //取得登入名稱
                HttpContext.Current.Session["Login_UserName"] = listAttr[1];
                //取得登入帳號
                HttpContext.Current.Session["Login_UserID"] = listAttr[2];
                //取得AD GUID
                HttpContext.Current.Session["Login_GUID"] = listAttr[3];
                //取得登入者所屬群組(ArrayList)
                Guid objectGuid = new Guid(listAttr[3]);
                ArrayList aryGroup = ADService.getGroupGUIDFromGUID(objectGuid);
                HttpContext.Current.Session["Login_UserGroups"] = aryGroup;
            }
        }
    }

    /// <summary>
    /// AD登入驗證 - 手動輸入帳密
    /// </summary>
    private void CheckAD_Input(string SID)
    {
        //取得登入相關資訊
        if (string.IsNullOrEmpty(SID))
        {
            //找不到此SID, 導向登入錯誤頁
            Response.Redirect(ErrPage("1001"));
            return;
        }
        else
        {
            //取得屬性值(Sid / DisplayName / AccountName / Guid / 帳戶類型)
            StringCollection listAttr = ADService.getAttributesFromSID(SID);
            if (listAttr == null)
            {
                //找不到此SID, 導向登入錯誤頁
                Response.Redirect(ErrPage("1002"));
                return;
            }
            else
            {
                //取得登入名稱
                HttpContext.Current.Session["Login_UserName"] = listAttr[1];
                //取得登入帳號
                HttpContext.Current.Session["Login_UserID"] = listAttr[2];
                //取得AD GUID
                HttpContext.Current.Session["Login_GUID"] = listAttr[3];
                //取得登入者所屬群組(ArrayList)
                Guid objectGuid = new Guid(listAttr[3]);
                ArrayList aryGroup = ADService.getGroupGUIDFromGUID(objectGuid);
                HttpContext.Current.Session["Login_UserGroups"] = aryGroup;
            }
        }
    }

    string ErrPage(string ErrMsg)
    {
        return "{0}ServiceLoginFail/{1}/".FormatThis(Application["WebUrl"].ToString(), ErrMsg);
    }
}