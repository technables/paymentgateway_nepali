<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="eCom_Authorize.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Card Number:<asp:TextBox ID="txtCartNumber" runat="server"></asp:TextBox>
        </div>
        <div>
            Expiration Date:<asp:TextBox ID="txtExpirationDate" runat="server"></asp:TextBox>
        </div>
        <div>
            CVV Number:<asp:TextBox ID="txtCVVNumber" runat="server"></asp:TextBox>
        </div>
        <div>
            <asp:Button ID="btnAuthorize" runat="server" Text="Pay with Authorize.Net" OnClick="btnAuthorize_Click" />
        </div>
    </form>
</body>
</html>
