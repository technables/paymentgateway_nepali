<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NCHL.aspx.cs" Inherits="eCom_NCHL.NCHL" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form action="https://demo.connectips.com/connectipswebgw/loginpage" method="post">
        <br>
        MERCHANT ID
                <input type="text" name="MERCHANTID" id="MERCHANTID" value="22" />
        <br>
        APP ID<input type="text" name="APPID" id="APPID" value="MER-22-APP-1" />
        <br>
        APP NAME
                <input type="text" name="APPNAME" id="APPNAME" value="Brain Digit Pvt Ltd" />
        <br>
        TXN ID
                <input type="text" name="TXNID" id="TXNID" value="123456" />
        <br>
        TXN DATE
                <input type="text" name="TXNDATE" id="TXNDATE" value="08-10-2018" />
        <br>
        TXN CRNCY
                <input type="text" name="TXNCRNCY" id="TXNCRNCY" value="NPR" />
        <br>
        TXN AMT
                <input type="text" name="TXNAMT" id="TXNAMT" value="1000" />
        <br>
        REFERENCE ID
                <input type="text" name="REFERENCEID" id="REFERENCEID" value="1.2.4" />
        <br>
        REMARKS
                <input type="text" name="REMARKS" id="REMARKS" value="123455" />
        <br>
        PARTICULARS
                <input type="text" name="PARTICULARS" id="PARTICULARS" value="123455" />
        <br>
        TOKEN
                <input type="text" name="TOKEN" id="TOKEN" runat="server" value="I6AgTZTEMf0Y4RPMvOdC8A8IkOhdqya0dGjY9JnbEYYiILxzk8FLA4sRq2QAE6/xMQhcvVtckQFp JkqxwwBOsUHw/lpvqUkKJdJyssWpi1CKF64DDYPn/OckLJmqVXnKCfx3Nkn8rPHlpFnkbk57SZl0ydKL9yKpezIQ0I9/C8tR6cazNo5YkA0TRfv5k21o 4hg0TWAb2uU7mXE9exsxHvtiz2FOoUamzQTXaHioGn83VjsXMkUrcecoAdPtg8//F0V4bHCImUTGz1Gm3CXCg KlcVFZhhGP861AvyZcQKmRwHFqDOTBt9KH362rmKoUMxPxHuEDr1i68ZA6Gxqw==" />
        <br>
        <input type="submit" value="Submit">
    </form>
</body>
</html>
