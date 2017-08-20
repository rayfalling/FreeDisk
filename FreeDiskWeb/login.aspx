<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="FreeDiskWeb.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Login-FreeDisk</title>
    <link href="css/style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-3.1.1.min.js"></script>
    <script src="Scripts/main.js"></script>
</head>
<body class="js-fullheight">
    <form id="form1" runat="server">
        <h1>FreeDisk</h1>
        <div class="login-form">
            <div class="avtar">
                <asp:Image src="images/avtar.png"  runat="server" id="headimg"/>
            </div>
            <asp:TextBox type="text" CssClass="text" Text="Username" onfocus="if(this.value == 'Username')this.value = '';" onblur="if (this.value == '') {this.value = 'Username';}" runat="server" OnTextChanged="LoadImg" />
            <asp:TextBox type="password" CssClass="password" Text="Password"  onfocus="if(this.value == 'Password')this.value = '';" onblur="if (this.value == '') {this.value = 'Password';}" runat="server" />
            <div class="signin">
                <asp:Button Text="Login" runat="server" OnClick="Login" />
            </div>
        </div>
    </form>
</body>
</html>
