<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Backoffice.aspx.cs" Inherits="SubtitleBackoffice.Backoffice" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>POOQ 다중언어 자막 관리자</title>
    <script type="text/javascript" src="Encoder.js"></script>

    <script type="text/javascript">
        function escapeHTML(inVal) {
            Encoder.EncodeType = "entity";
            var decoded = Encoder.htmlDecode(inVal);
            return decoded.replace(/</g, '&lt;').replace(/>/g, '&gt;');
        }

        function unescapeHTML(inVal) {
            return inVal.stripTags().replace(/&gt;/g, '>').replace(/&amp;/g, '&');
        }

        function ProcessNewEntry(s, e) {
            var sObj = ASPxClientGridView.Cast(s);
            sObj.GetRowValues(e.visibleIndex, "ID;Name;CategoryCode", OnGetRowValues);
        }

        function OnGetRowValues(values) {
            ContentsID.SetValue(values[0]);
            ContentsName.SetValue(values[1]);
            CategoryCode.SetValue(values[2]);
        }

        function CheckUpdate(s, e) {
            // 입력 데이타 검사
            if (ContentsID.GetValue() == null) {
                alert('컨텐츠ID를 검색하여 입력하세요.');
                return;
            }
            if (ContentsName.GetValue() == null) {
                alert('컨텐츠명을 검색하여 입력하세요.');
                return;
            }
            if (EpisodeNumber.GetValue() == null) {
                alert('회차번호를 입력하세요.');
                return;
            }
            if (CategoryCode.GetValue() == null) {
                alert('종류코드를 선택하세요.');
                return;
            }
            if (CountryCode.GetValue() == null) {
                alert('언어코드를 선택하세요.');
                return;
            }
            if (subtitle.GetValue() == null) {
                alert('자막을 입력하세요');
                return;
            }
        }
        function ProcessSubtitle(s, e) {
            var theText = subtitle.GetValue();
            subtitle.SetValue(escapeHTML(theText));
        }

        function ProcessClearButton(s, e) {
            var row = MainGrid.GetRow(0);
            if (MainGrid._isGroupRow(row) != true) {
                e.processOnServer = false; //Stop processing on server
            }
        }
    </script>

    <style type="text/css">
        .auto-style1
        {
            width: 110px;
        }

        .auto-style2
        {
            width: 110px;
        }

        .auto-style3
        {
            width: 110px;
        }

        .auto-style4
        {
            width: 558px;
        }

        .auto-style5
        {
            height: 10px;
        }

        .auto-style6
        {
            width: 110px;
        }
    </style>

</head>
<body style="background-color: bisque">
    <form id="form1" runat="server">

        <!-- 제목위치 //-->
        <div id="Header" style="background-color: bisque">
            <img src="http://www.pooq.co.kr/images/common/img_logo01.png" />
            <b>다중언어 자막 관리자
                <asp:Label ID="versionPlaceholder" runat="server" Text="0.1"></asp:Label></b>
            <p>
                <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="신규 추가: [콘텐츠추가] 클릭, 우측상단 목록에서 검색 후 '선택' 클릭, 나머지 정보 입력후 [업데이트]"></dx:ASPxLabel>
                <br />
                <dx:ASPxLabel ID="ASPxLabel2" runat="server" Text="회차,언어 추가: 기존 콘텐츠 목록에서 [복사]클릭, 정보 업데이트 및 수정 후 [업데이트]"></dx:ASPxLabel>
                <dx:ASPxTextBox ID="TextBox_Message2" runat="server" Width="584px" Height="24px" ReadOnly="True" Theme="Office2003Blue" ClientIDMode="Inherit"></dx:ASPxTextBox>
            </p>

        </div>


        <!-- 검색그리드 //-->
        <div id="div_search" style="position: absolute; top: 10px; right: 5px;">
            <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource_search" EnableTheming="True" KeyFieldName="ID" Theme="Office2003Silver">
                <ClientSideEvents CustomButtonClick="function(s, e) {ProcessNewEntry(s,e);}" />
                <Columns>
                    <dx:GridViewCommandColumn Caption="콘텐츠 검색" VisibleIndex="0">
                        <ClearFilterButton Visible="True" Text="검색리샛">
                        </ClearFilterButton>
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton ID="_SelectContent" Text="선택">
                            </dx:GridViewCommandColumnCustomButton>
                        </CustomButtons>
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn FieldName="ID" ReadOnly="True" VisibleIndex="1" Caption="콘텐츠ID">
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="2" Caption="콘텐츠명">
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn Caption="종류코드" FieldName="CategoryCode" VisibleIndex="3" Width="50px">
                    </dx:GridViewDataTextColumn>
                </Columns>
                <Settings ShowFilterRow="True" />
                <SettingsPager PageSize="4"></SettingsPager>
            </dx:ASPxGridView>
            <asp:SqlDataSource ID="SqlDataSource_search" runat="server" ConnectionString="<%$ ConnectionStrings:subtitleConnectionString %>" SelectCommand="SELECT [ID], [Name], [CategoryCode] FROM [List_Content]"></asp:SqlDataSource>
        </div>


        <!-- 메뉴 위치 //-->
        <div id="div_menu" style="background-color: bisque">
            <table style="width: 100%;">
                <tr>
                    <td colspan="1" class="auto-style1">
                        <dx:ASPxButton ID="ASPxButton_Insert" runat="server" Text="콘텐츠 추가" Theme="Office2003Blue" OnClick="ASPxButton_Insert_Click" Width="120px"></dx:ASPxButton>
                    </td>
                    <td class="auto-style2">
                        <dx:ASPxButton ID="ASPxButton_Clear" runat="server" Text="그룹 클리어" Theme="Office2003Blue" OnClick="ASPxButton_Clear_Click" AutoPostBack="false" EnableTheming="True" Width="120px">
                            <ClientSideEvents Click="function(s, e) {ProcessClearButton(s,e)}" />
                        </dx:ASPxButton>
                    </td>
                    <td class="auto-style3">
                        <dx:ASPxButton ID="ASPxButton_Reset" runat="server" Text="관리자 리샛" OnClick="ASPxButton_Reset_Click" Theme="Office2003Blue" CausesValidation="false" Width="120px"></dx:ASPxButton>
                    </td>
                    <td class="auto-style6">
                        <dx:ASPxButton ID="ASPxButton_UpdateContentsList" runat="server" Text="*콘텐츠 목록갱신" EnableTheming="True" OnClick="ASPxButton_UpdateContentsList_Click" Theme="Office2003Blue" Width="120px">
                            <ClientSideEvents Click="function(s, e) {
	alert('서버에서 콘텐츠 목록을 다시 받습니다.\n\r잠시만 기다려 주세요.');
    TextBox_Message2.SetValue('목록 받는 중...');
}" />
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="ASPxButton_GenJSON" runat="server" Text="*프로그램 목록 생성" Theme="Office2003Blue" Width="133px" OnClick="ASPxButton_GenJSON_Click" AutoPostBack="false">
                        </dx:ASPxButton>
                    </td>
                </tr>
            </table>
        </div>


        <!-- 메인 그리드 위치 //-->
        <div id="div_main" style="background-color: bisque">

            <dx:ASPxGridView ID="ASPxGridView2" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource_Main" EnableTheming="True" KeyFieldName="ID" Theme="Office2003Blue" Width="100%" ClientInstanceName="MainGrid">
                <Columns>
                    <dx:GridViewCommandColumn Caption="작업" VisibleIndex="0" Width="130px" ButtonType="Button">
                        <EditButton Text="편집" Visible="True">
                        </EditButton>
                        <DeleteButton Text="삭제" Visible="True">
                        </DeleteButton>
                        <CancelButton Text="취소">
                        </CancelButton>
                        <UpdateButton Text="업데이트">
                        </UpdateButton>
                        <ClearFilterButton Visible="True" Text="필터해제">
                        </ClearFilterButton>
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton ID="_CommandCopy" Text="복사">
                            </dx:GridViewCommandColumnCustomButton>
                        </CustomButtons>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn Caption="레코드ID" FieldName="ID" ReadOnly="True" VisibleIndex="1" Width="50px">
                        <EditFormSettings Visible="False" />
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn Caption="컨텐츠ID" FieldName="ContentID" VisibleIndex="2" Width="140px">
                        <PropertiesTextEdit ClientInstanceName="ContentsID"></PropertiesTextEdit>
                        <EditFormSettings ColumnSpan="1" RowSpan="1"></EditFormSettings>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn Caption="컨텐츠명" FieldName="Name" VisibleIndex="3" Width="140px">
                        <PropertiesTextEdit ClientInstanceName="ContentsName"></PropertiesTextEdit>
                        <EditFormSettings ColumnSpan="3" RowSpan="1"></EditFormSettings>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataSpinEditColumn Caption="회차" FieldName="EpisodeNumber" VisibleIndex="4" Width="30px">
                        <PropertiesSpinEdit DisplayFormatString="g" ClientInstanceName="EpisodeNumber">
                        </PropertiesSpinEdit>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataSpinEditColumn>
                    <dx:GridViewDataComboBoxColumn Caption="종류코드" FieldName="CategoryCode" VisibleIndex="5" Width="40px" PropertiesComboBox-ClientInstanceName="CategoryCode">
                        <PropertiesComboBox>
                            <Items>
                            </Items>
                        </PropertiesComboBox>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn Caption="언어코드" FieldName="CountryCode" VisibleIndex="6" Width="40px" PropertiesComboBox-ClientInstanceName="CountryCode">
                        <PropertiesComboBox>
                            <Items>
                            </Items>
                        </PropertiesComboBox>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn Caption="상태코드" FieldName="StatusCode" VisibleIndex="7" Width="40px" PropertiesComboBox-ClientInstanceName="StatusCode" ReadOnly="True">
                        <PropertiesComboBox>
                            <Items>
                                <dx:ListEditItem Text="초기" Value="INI" />
                                <dx:ListEditItem Text="작업중" Value="PRC" />
                                <dx:ListEditItem Text="서비스" Value="DPL" />
                            </Items>
                        </PropertiesComboBox>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataDateColumn Caption="업데이트날짜" FieldName="UpdateDate" ReadOnly="True" VisibleIndex="8" Width="75px">
                        <PropertiesDateEdit ClientInstanceName="UpdateDate" DisplayFormatString="">
                        </PropertiesDateEdit>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataDateColumn>
                    <dx:GridViewDataTextColumn FieldName="URL" VisibleIndex="9" PropertiesTextEdit-ClientInstanceName="URL" ReadOnly="True" Width="80px">
                        <PropertiesTextEdit ClientInstanceName="URL"></PropertiesTextEdit>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataMemoColumn Caption="자막" FieldName="subtitle" VisibleIndex="10" PropertiesMemoEdit-ClientInstanceName="subtitle" Width="200px">
                        <PropertiesMemoEdit Rows="12">
                            <ClientSideEvents Validation="function(s, e) {CheckUpdate(s,e);ProcessSubtitle(s, e);}" />
                        </PropertiesMemoEdit>
                        <EditFormSettings ColumnSpan="2" RowSpan="16"></EditFormSettings>
                        <CellStyle VerticalAlign="Top">
                        </CellStyle>
                    </dx:GridViewDataMemoColumn>
                </Columns>
                <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                <SettingsEditing EditFormColumnCount="4" />
            </dx:ASPxGridView>
            <asp:SqlDataSource ID="SqlDataSource_Main" runat="server" ConnectionString="<%$ ConnectionStrings:subtitleConnectionString %>" DeleteCommand="DELETE FROM [Subtitle] WHERE [ID] = @ID" InsertCommand="INSERT INTO [Subtitle] ([ContentID], [Name], [EpisodeNumber], [CategoryCode], [CountryCode], [StatusCode], [UpdateDate], [URL], [subtitle]) VALUES (@ContentID, @Name, @EpisodeNumber, @CategoryCode, @CountryCode, @StatusCode, GETDATE(), @URL, @subtitle)" SelectCommand="SELECT * FROM [Subtitle] ORDER BY [ID] DESC" UpdateCommand="UPDATE [Subtitle] SET [ContentID] = @ContentID, [Name] = @Name, [EpisodeNumber] = @EpisodeNumber, [CategoryCode] = @CategoryCode, [CountryCode] = @CountryCode, [StatusCode] = @StatusCode, [UpdateDate] = GETDATE(), [URL] = @URL, [subtitle] = @subtitle WHERE [ID] = @ID">
                <DeleteParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                </DeleteParameters>
                <InsertParameters>
                    <asp:Parameter Name="ContentID" Type="String" />
                    <asp:Parameter Name="Name" Type="String" />
                    <asp:Parameter Name="EpisodeNumber" Type="String" />
                    <asp:Parameter Name="CategoryCode" Type="String" />
                    <asp:Parameter Name="CountryCode" Type="String" />
                    <asp:Parameter Name="StatusCode" Type="String" />
                    <asp:Parameter DbType="DateTime2" Name="UpdateDate" />
                    <asp:Parameter Name="URL" Type="String" />
                    <asp:Parameter Name="subtitle" Type="String" />
                </InsertParameters>
                <UpdateParameters>
                    <asp:Parameter Name="ContentID" Type="String" />
                    <asp:Parameter Name="Name" Type="String" />
                    <asp:Parameter Name="EpisodeNumber" Type="String" />
                    <asp:Parameter Name="CategoryCode" Type="String" />
                    <asp:Parameter Name="CountryCode" Type="String" />
                    <asp:Parameter Name="StatusCode" Type="String" />
                    <asp:Parameter DbType="DateTime2" Name="UpdateDate" />
                    <asp:Parameter Name="URL" Type="String" />
                    <asp:Parameter Name="subtitle" Type="String" />
                    <asp:Parameter Name="ID" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>

        </div>
        <dx:ASPxLabel ID="ASPxLabel_ClientIP" runat="server" Text="클라이언트 IP:" Theme="Office2003Blue">
        </dx:ASPxLabel>
    </form>
</body>
</html>
