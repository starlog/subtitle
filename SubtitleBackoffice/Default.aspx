<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SubtitleBackoffice.Default" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxMenu" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
    <style type="text/css">
        body {
            font-family: 'Malgun Gothic';
            font-size: 10pt;
        }

        #getCategory_Param1 {
            width: 228px;
        }

        .auto-style1 {
            width: 101px;
        }

        .auto-style2 {
            width: 216px;
        }

        .auto-style3 {
            width: 61px;
        }

        .auto-style4 {
            width: 291px;
        }

        .auto-style6 {
            width: 69px;
        }

        .auto-style7 {
            width: 55px;
        }

        .auto-style8 {
            width: 156px;
        }

        .auto-style9 {
            width: 84px;
        }

        .auto-style10 {
            width: 80px;
        }

        .auto-style11 {
            width: 213px;
        }

        .auto-style12 {
            width: 68px;
        }

        .auto-style13 {
            width: 167px;
        }
        .auto-style14 {
            width: 293px;
        }
        </style>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>
                <dx:ASPxMenu ID="ASPxMenu1" runat="server" EnableTheming="True" OnItemClick="ASPxMenu1_ItemClick" RenderMode="Lightweight" Theme="Aqua">
                    <Items>
                        <dx:MenuItem Text="백오피스 보기">
                        </dx:MenuItem>
                    </Items>
                </dx:ASPxMenu>
            </h1>
            <h1>다중자막 서버
            </h1>
            <br />

            Version
            <dx:ASPxLabel ID="ASPxLabel_version" runat="server" Text="(버전번호)" Theme="Office2003Blue">
            </dx:ASPxLabel>
            <br />
            <p style="text-align: right">
                (주)오픈 플랫폼<br />
                September, 2013<br />
            </p>
            <h2>Command Reference</h2>
            <h3>URL Structure</h3>
            <hr />
            단일 URL로 명령을 전송하고 http redirection으로 응답을 받는다.
            <br />
            BaseURL?Cmd=XXXX&Param1=XXX&Param2=XXX…..
            <br />
            Cmd 파라메터는 반드시 존재하여야 한다. Param1…Paramn 파라메터의 존재 여부 및 의미는 명령에 따라 다르다.
            <br />
            <br />
            BaseURL = Processor.aspx
            <br />
            <br />
            문자 Encoding은 UTF-8이다
            <br />
            <br />
            <h3>Cmd:GetCategory</h3>
            <hr />
            카테고리에 해당하는, 다중 자막이 존재하는 콘텐츠 목록을 JSON파일로 다운로드 한다. 다운로드 되는 JSON파일의 이름은 CategoryList_&lt;카테고리명&gt;.json 이다 
            <br />
            <br />
            CategoryCode: 카테고리 이름 지정
            <br />
            Drama, Sisa, Entertainment, News
            (주의:Version 0.36이후부터는 이 카데고리 이름이 Pooq API에 따라 수정,첨삭될수 있음)
            <br />
            <table style="width: 100%; background-color: bisque">
                <tr>
                    <td class="auto-style1">카데고리 이름:</td>
                    <td class="auto-style2">
                        <dx:ASPxComboBox ID="ASPxComboBox_GetCategory" runat="server" EnableTheming="True" Theme="Office2003Blue" ClientInstanceName="ASPxComboBox_GetCategory" SelectedIndex="0">
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxButton ID="ASPxButton_GetCategory" runat="server" Text="테스트" OnClick="ASPxButton_GetCategory_Click" Theme="Office2003Blue"></dx:ASPxButton>
                    </td>
                </tr>
            </table>
            <p><a href="Processor.aspx?Cmd=GetCategory&CategoryCode=Drama">Processor.aspx?Cmd=GetCategory&CategoryCode=Drama</a></p>
            <br />
            <h3>Cmd: GetSubtitleList</h3>
            <hr />
            특정 콘텐츠, 특정 회차에 대한 특정 언어의 자막이미지 파일의 목록을 JSON파일 형태로 다운로드 한다. 
            <br />
            callback 파라메터를 전달하면 그 이름으로, 전달하지 않으면 Defalt callback 이름이 사용된다<br />
            <br />
            ContentID: Contents ID (예: “S01_V0000379136”) 
            <br />
            EpisodeNumber: 회차번호 (예: 1) 
            <br />
            CountryCode: ISO 국가코드 (“KOR”, “CHN”, “USA”, “JPN”, “VNM”) 
            <br />
            <font color="red">Device: 접속하는 장비의 형태 (&quot;PC&quot;, &quot;TV&quot;)</font><br />
            <table style="width: 100%; background-color: bisque">
                <tr>
                    <td class="auto-style3">콘텐츠ID:</td>
                    <td class="auto-style4">

                        <dx:ASPxComboBox ID="ASPxComboBox_GetSubtitleList_ID" runat="server" DataSourceID="SqlDataSource1" EnableTheming="True" Height="19px" Theme="Office2003Blue" Width="268px" ValueField="ContentID">
                            <Columns>
                                <dx:ListBoxColumn FieldName="Name" />
                                <dx:ListBoxColumn FieldName="ContentID" />
                            </Columns>
                        </dx:ASPxComboBox>

                    </td>
                    <td class="auto-style7">회차번호</td>
                    <td class="auto-style6">
                        <dx:ASPxSpinEdit ID="ASPxSpinEdit_EpisodeNumber" runat="server" Height="18px" Number="1" Theme="Office2003Blue" Width="57px" />
                    </td>
                    <td class="auto-style7">언어코드</td>
                    <td class="auto-style8">
                        <dx:ASPxComboBox ID="ASPxComboBox_GetSubtitleList_CountryCode" runat="server" EnableTheming="True" Theme="Office2003Blue" SelectedIndex="0" Height="19px" Width="125px" DataSourceID="SqlDataSource2" ValueField="Code">
                            <Columns>
                                <dx:ListBoxColumn FieldName="Desc" />
                                <dx:ListBoxColumn FieldName="Code" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxButton ID="ASPxButton_getSubtitleList" runat="server" Text="테스트" Theme="Office2003Blue" OnClick="ASPxButton_getSubtitleList_Click"></dx:ASPxButton>
                    </td>
                </tr>
            </table>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:subtitleConnectionString %>" SelectCommand="SELECT DISTINCT top 10 [ContentID], [Name] FROM [Subtitle]"></asp:SqlDataSource>

            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:subtitleConnectionString %>" SelectCommand="SELECT [Code], [Desc] FROM [Code_Country]"></asp:SqlDataSource>
            <br />
            <p><a href="Processor.aspx?Cmd=GetSubtitleList&ContentID=M_1002692100000100000&EpisodeNumber=1&CountryCode=KOR&Device=PC">Processor.aspx?Cmd=GetSubtitleList&ContentID=M_1002692100000100000&EpisodeNumber=1&CountryCode=KOR&Device=PC</a></p>
            <p><a href="Processor.aspx?Cmd=GetSubtitleList&ContentID=M_1002692100000100000&EpisodeNumber=1&CountryCode=KOR&Device=PC&callback=MyCallback">Processor.aspx?Cmd=GetSubtitleList&ContentID=M_1002692100000100000&EpisodeNumber=1&CountryCode=KOR&Device=PC&callback=MyCallback</a></p>
            <br />
            <h3>Cmd: GetSubtitleDirectory</h3>
            <hr />
            특정 콘텐츠, 특정 회차에 대한 특정 언어의 자막이미지 파일이 위치한 폴더의 URL을 리턴한다.
            <br />
            callback 파라메터를 전달하면 그 이름으로, 전달하지 않으면 Defalt callback 이름이 사용된다<br />
            <br />
            <br />
            Param1: Contents ID (예: “S01_V0000379136”) 
            <br />
            Param2: 회차번호 (예: 1) 
            <br />
            Param3: ISO 국가코드 (“KOR”, “CHN”, “USA”, “JPN”, “VNM”) 
            <br />
            <table style="width: 100%; background-color: bisque">
                <tr>
                    <td class="auto-style3">콘텐츠ID:</td>
                    <td class="auto-style4">

                        <dx:ASPxComboBox ID="ASPxComboBox_GetSubtitleDirectory_ContentID" runat="server" DataSourceID="SqlDataSource1" EnableTheming="True" Height="19px" Theme="Office2003Blue" Width="268px" ValueField="ContentID">
                            <Columns>
                                <dx:ListBoxColumn FieldName="Name" />
                                <dx:ListBoxColumn FieldName="ContentID" />
                            </Columns>
                        </dx:ASPxComboBox>

                    </td>
                    <td class="auto-style9">회차번호</td>
                    <td class="auto-style6">
                        <dx:ASPxSpinEdit ID="ASPxSpinEdit_GetSubtitleDirectory_EpisodeNumber" runat="server" Height="26px" Number="1" Theme="Office2003Blue" Width="72px" />
                    </td>
                    <td class="auto-style7">언어코드</td>
                    <td class="auto-style8">
                        <dx:ASPxComboBox ID="ASPxComboBox_GetSubtitleDirectory_CountryCode" runat="server" EnableTheming="True" Theme="Office2003Blue" SelectedIndex="0" Height="19px" Width="125px" DataSourceID="SqlDataSource2" ValueField="Code">
                            <Columns>
                                <dx:ListBoxColumn FieldName="Desc" />
                                <dx:ListBoxColumn FieldName="Code" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxButton ID="ASPxButton__GetSubtitleDirectory_Button" runat="server" Text="테스트" Theme="Office2003Blue" OnClick="ASPxButton__GetSubtitleDirectory_Button_Click"></dx:ASPxButton>
                    </td>
                </tr>
            </table>
            <br />
            <p><a href="Processor.aspx?Cmd=GetSubtitleDirectory&ContentID=S01_V0000379136&EpisodeNumber=1&CountryCode=VNM">Processor.aspx?Cmd=GetSubtitleDirectory&ContentID=S01_V0000379136&EpisodeNumber=1&CountryCode=VNM</a></p>
            <p><a href="Processor.aspx?Cmd=GetSubtitleDirectory&ContentID=S01_V0000379136&EpisodeNumber=1&CountryCode=VNM&callback=MyCallback">Processor.aspx?Cmd=GetSubtitleDirectory&ContentID=S01_V0000379136&EpisodeNumber=1&CountryCode=VNM&callback=MyCallback</a></p>
            <br />
            <h3>Cmd: GetCategoryList</h3>
            <hr />
            카테고리 목록을 리턴한다.
            <br />
            callback 파라메터를 전달하면 그 이름으로, 전달하지 않으면 Defalt callback 이름이 사용된다<br />
            <p><a href="Processor.aspx?Cmd=GetCategoryList">Processor.aspx?Cmd=GetCategoryList</a></p>
            <p><a href="Processor.aspx?Cmd=GetCategoryList&callback=MyCallbackName">Processor.aspx?Cmd=GetCategoryList&callback=MyCallbackName</a></p>
            <br />
            <br />
            <h3>Cmd: V2GetProgramList</h3>
            <hr />
            해당 카데고리의 프로그램 목록을 리턴한다.
            <br />
            callback 파라메터를 전달하면 그 이름으로, 전달하지 않으면 Defalt callback 이름이 사용된다<br />
            <br />
            <br />
            <table style="width: 100%; background-color: bisque">
                <tr>
                    <td class="auto-style10">카데고리명:
                    </td>
                    <td class="auto-style11">
                        <dx:ASPxComboBox ID="ASPxComboBox_V2GetProgramList" runat="server" EnableTheming="True" Theme="Office2003Blue" ClientInstanceName="ASPxComboBox_V2GetProgramList" SelectedIndex="0">
                        </dx:ASPxComboBox>
                    </td>
                    <td class="auto-style12">Skip:</td>
                    <td class="auto-style8">
                        <dx:ASPxTextBox ID="ASPxTextBox_V2_GetProgramList_Skip" runat="server" Theme="Office2003Blue" Width="170px">
                        </dx:ASPxTextBox>
                    </td>
                    <td class="auto-style10">Take:</td>
                    <td class="auto-style13">
                        <dx:ASPxTextBox ID="ASPxTextBox_V2_GetProgramList_Take" runat="server" Theme="Office2003Blue" Width="170px">
                        </dx:ASPxTextBox>
                    </td>
                    <td>
                        <dx:ASPxButton ID="ASPxButton_Test_V2GetProgramList" runat="server" OnClick="ASPxButton_Test_V2GetProgramList_Click" Text="테스트" Theme="Aqua">
                        </dx:ASPxButton>

                    </td>
                </tr>
            </table>
            <p><a href="Processor.aspx?Cmd=V2GetProgramList&menu-id=Drama&skip=1&take=1&callback=MyCallbackName">Processor.aspx?Cmd=V2GetProgramList&menu-id=Drama&skip=1&take=1&callback=MyCallbackName</a></p>
            <br />
            <br />
            <h3>Cmd: V2GetEpisodeList</h3>
            <hr />
            해당 카데고리의 자막이 포함된 회차 목록을 리턴한다.
            <br />
            callback 파라메터를 전달하면 그 이름으로, 전달하지 않으면 Defalt callback 이름이 사용된다<br />
            <br />
            <br />
            <table style="width: 100%; background-color: bisque">
                <tr>
                    <td class="auto-style14">
                        <dx:ASPxComboBox ID="ASPxComboBox_V2_GetEpisodeList" runat="server" DataSourceID="SqlDataSource1" EnableTheming="True" Height="19px" Theme="Office2003Blue" Width="268px" ValueField="ContentID">
                            <Columns>
                                <dx:ListBoxColumn FieldName="Name" />
                                <dx:ListBoxColumn FieldName="ContentID" />
                            </Columns>
                        </dx:ASPxComboBox>

                    </td>
                    <td class="auto-style12">Skip:</td>
                    <td class="auto-style8">
                        <dx:ASPxTextBox ID="ASPxTextBox_V2_GetEpisodeList_Skip" runat="server" Theme="Office2003Blue" Width="170px">
                        </dx:ASPxTextBox>
                    </td>
                    <td class="auto-style10">Take:</td>
                    <td class="auto-style13">
                        <dx:ASPxTextBox ID="ASPxTextBox_V2_GetEpisodeList_Take" runat="server" Theme="Office2003Blue" Width="170px">
                        </dx:ASPxTextBox>
                    </td>
                    <td>
                        <dx:ASPxButton ID="ASPxButton_V2_GetEpisodList" runat="server" OnClick="ASPxButton_V2_GetEpisodList_Click" Text="테스트" Theme="Aqua">
                        </dx:ASPxButton>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <p><a href="Processor.aspx?Cmd=V2GetEpisodeList&program-id=K01_T2012-0325&skip=1&take=1&callback=MyCallbackName">Processor.aspx?Cmd=V2GetEpisodeList&program-id=K01_T2012-0325&skip=1&take=1&callback=MyCallbackName</a></p>
            <br />
            <br />
            <h3>Cmd: V2GetLatestEpisodeList</h3>
            <hr />
            날짜기준으로 최신 회차 목록을 프로그램에 관계 없이 리턴한다. (갯수는 고정임)
            <br />
            callback 파라메터를 전달하면 그 이름으로, 전달하지 않으면 Defalt callback 이름이 사용된다<br />
            <br />
            <p><a href="Processor.aspx?Cmd=V2GetLatestEpisodeList&callback=MyCallbackName">Processor.aspx?Cmd=V2GetLatestEpisodeList&callback=MyCallbackName</a></p>
            <br />
            <br />

        </div>
    </form>
</body>
</html>
