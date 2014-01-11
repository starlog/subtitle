<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Install.aspx.cs" Inherits="SubtitleBackoffice.Install" %>

<%@ Register assembly="DevExpress.Web.v13.1, Version=13.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxMenu" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.v13.1, Version=13.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <dx:ASPxMenu ID="ASPxMenu1" runat="server" EnableTheming="True" RenderMode="Lightweight" Theme="Office2003Blue">
            <Items>
                <dx:MenuItem Text="데이타베이스">
                    <Items>
                        <dx:MenuItem Text="테이블 생성">
                        </dx:MenuItem>
                        <dx:MenuItem Text="기본코드 삽입">
                        </dx:MenuItem>
                    </Items>
                </dx:MenuItem>
                <dx:MenuItem Text="JSON 파일">
                    <Items>
                        <dx:MenuItem Text="프로그램목록 JSON">
                        </dx:MenuItem>
                    </Items>
                </dx:MenuItem>
            </Items>
        </dx:ASPxMenu>
    
    </div>
        <br />
        <dx:ASPxMemo ID="ASPxMemo1" runat="server" Height="300px" Theme="Office2003Blue" Width="700px">
        </dx:ASPxMemo>
    </form>
</body>
</html>
