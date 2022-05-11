using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// 自定Form
/// </summary>
public class fn_FormPost
{
    private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
    public string Url = "";
    public string Method = "post";
    public string FormName = "myHiddenForm";
    public string FormTarget = "_self";

    public void Add(string name, string value)
    {
        Inputs.Add(name, value);
    }

    public void Post()
    {
        HttpContext.Current.Response.Clear();

        HttpContext.Current.Response.Write("<html><head>");
        HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
        HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" target=\"{3}\" >", FormName, Method, Url, FormTarget));
        for (int i = 0; i <= Inputs.Keys.Count - 1; i++)
        {
            HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
        }
        HttpContext.Current.Response.Write("</form>");
        HttpContext.Current.Response.Write("</body></html>");

        HttpContext.Current.Response.End();
    }

}

/*
  [如何使用]

fn_FormPost resp = new fn_FormPost();

//Url
resp.Url = Param_Url;

//token
resp.Add("token", mySecurity.Encrypt(myCode));

//Json Data
resp.Add("data", Cryptograph.Encrypt(output));

resp.Post();

*/