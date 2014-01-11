<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="SubtitleBackoffice.Settings" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxClasses" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>다중자막 시스템 설정</title>
</head>
<body style="background-color: bisque">
    <form id="form1" runat="server">
        <div>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:subtitleConnectionString %>" DeleteCommand="DELETE FROM [Code_Category] WHERE [Code] = @Code" InsertCommand="INSERT INTO [Code_Category] ([Code], [Desc], [Pooq_Code]) VALUES (@Code, @Desc, @Pooq_Code)" SelectCommand="SELECT * FROM [Code_Category]" UpdateCommand="UPDATE [Code_Category] SET [Desc] = @Desc, [Pooq_Code] = @Pooq_Code WHERE [Code] = @Code">
                <DeleteParameters>
                    <asp:Parameter Name="Code" Type="String" />
                </DeleteParameters>
                <InsertParameters>
                    <asp:Parameter Name="Code" Type="String" />
                    <asp:Parameter Name="Desc" Type="String" />
                    <asp:Parameter Name="Pooq_Code" Type="String" />
                </InsertParameters>
                <UpdateParameters>
                    <asp:Parameter Name="Desc" Type="String" />
                    <asp:Parameter Name="Pooq_Code" Type="String" />
                    <asp:Parameter Name="Code" Type="String" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </div>
        <br />
        <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="3" EnableTheming="True" RenderMode="Lightweight" Theme="Office2003Blue" Width="570px">
            <TabPages>
                <dx:TabPage Text="카데고리 코드 편집">
                    <ContentCollection>
                        <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" EnableTheming="True" HeaderText="카데고리 코드" Theme="Office2003Blue" Width="550px">
                                <PanelCollection>
                                    <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                                        <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" KeyFieldName="Code" Theme="Office2003Blue">
                                            <Columns>
                                                <dx:GridViewCommandColumn ShowInCustomizationForm="True" VisibleIndex="0">
                                                    <EditButton Visible="True">
                                                    </EditButton>
                                                    <NewButton Visible="True">
                                                    </NewButton>
                                                    <DeleteButton Visible="True">
                                                    </DeleteButton>
                                                    <ClearFilterButton Visible="True">
                                                    </ClearFilterButton>
                                                </dx:GridViewCommandColumn>
                                                <dx:GridViewDataTextColumn FieldName="Code" ShowInCustomizationForm="True" VisibleIndex="1">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Desc" ShowInCustomizationForm="True" VisibleIndex="2">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Pooq_Code" ShowInCustomizationForm="True" VisibleIndex="3">
                                                </dx:GridViewDataTextColumn>
                                            </Columns>
                                            <Settings ShowFilterRow="True" />
                                        </dx:ASPxGridView>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
                <dx:TabPage Text="언어 코드 편집">
                    <ContentCollection>
                        <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" HeaderText="언어 코드" Theme="Office2003Blue" Width="550px">
                                <PanelCollection>
                                    <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                                        <dx:ASPxGridView ID="ASPxGridView2" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource2" KeyFieldName="Code" Theme="Office2003Blue">
                                            <Columns>
                                                <dx:GridViewCommandColumn ShowInCustomizationForm="True" VisibleIndex="0">
                                                    <EditButton Visible="True">
                                                    </EditButton>
                                                    <NewButton Visible="True">
                                                    </NewButton>
                                                    <DeleteButton Visible="True">
                                                    </DeleteButton>
                                                    <ClearFilterButton Visible="True">
                                                    </ClearFilterButton>
                                                </dx:GridViewCommandColumn>
                                                <dx:GridViewDataTextColumn FieldName="Code" ShowInCustomizationForm="True" VisibleIndex="1">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Desc" ShowInCustomizationForm="True" VisibleIndex="2">
                                                </dx:GridViewDataTextColumn>
                                            </Columns>
                                            <Settings ShowFilterRow="True" />
                                        </dx:ASPxGridView>
                                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:subtitleConnectionString %>" DeleteCommand="DELETE FROM [Code_Country] WHERE [Code] = @Code" InsertCommand="INSERT INTO [Code_Country] ([Code], [Desc]) VALUES (@Code, @Desc)" SelectCommand="SELECT * FROM [Code_Country]" UpdateCommand="UPDATE [Code_Country] SET [Desc] = @Desc WHERE [Code] = @Code">
                                            <DeleteParameters>
                                                <asp:Parameter Name="Code" Type="String" />
                                            </DeleteParameters>
                                            <InsertParameters>
                                                <asp:Parameter Name="Code" Type="String" />
                                                <asp:Parameter Name="Desc" Type="String" />
                                            </InsertParameters>
                                            <UpdateParameters>
                                                <asp:Parameter Name="Desc" Type="String" />
                                                <asp:Parameter Name="Code" Type="String" />
                                            </UpdateParameters>
                                        </asp:SqlDataSource>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
                <dx:TabPage Text="접근 보안 편집">
                    <ContentCollection>
                        <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                            <dx:ASPxGridView ID="ASPxGridView3" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource3" EnableTheming="True" KeyFieldName="ID" Theme="Office2003Blue">
                                <Columns>
                                    <dx:GridViewCommandColumn ShowInCustomizationForm="True" VisibleIndex="0">
                                        <EditButton Visible="True">
                                        </EditButton>
                                        <NewButton Visible="True">
                                        </NewButton>
                                        <DeleteButton Visible="True">
                                        </DeleteButton>
                                        <ClearFilterButton Visible="True">
                                        </ClearFilterButton>
                                    </dx:GridViewCommandColumn>
                                    <dx:GridViewDataTextColumn FieldName="ID" ShowInCustomizationForm="True" VisibleIndex="1">
                                        <EditFormSettings Visible="False" />
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataComboBoxColumn FieldName="Type" ShowInCustomizationForm="True" VisibleIndex="2">
                                        <PropertiesComboBox>
                                            <Items>
                                                <dx:ListEditItem Text="ID/PW" Value="ID" />
                                                <dx:ListEditItem Text="IP" Value="IP" />
                                            </Items>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataTextColumn FieldName="Param1" ShowInCustomizationForm="True" VisibleIndex="3">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Param2" ShowInCustomizationForm="True" VisibleIndex="4">
                                    </dx:GridViewDataTextColumn>
                                </Columns>
                                <Settings ShowFilterRow="True" />
                            </dx:ASPxGridView>
                            <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:subtitleConnectionString %>" DeleteCommand="DELETE FROM [Access] WHERE [ID] = @ID" InsertCommand="INSERT INTO [Access] ([Type], [Param1], [Param2]) VALUES (@Type, @Param1, @Param2)" SelectCommand="SELECT * FROM [Access]" UpdateCommand="UPDATE [Access] SET [Type] = @Type, [Param1] = @Param1, [Param2] = @Param2 WHERE [ID] = @ID">
                                <DeleteParameters>
                                    <asp:Parameter Name="ID" Type="Int32" />
                                </DeleteParameters>
                                <InsertParameters>
                                    <asp:Parameter Name="Type" Type="String" />
                                    <asp:Parameter Name="Param1" Type="String" />
                                    <asp:Parameter Name="Param2" Type="String" />
                                </InsertParameters>
                                <UpdateParameters>
                                    <asp:Parameter Name="Type" Type="String" />
                                    <asp:Parameter Name="Param1" Type="String" />
                                    <asp:Parameter Name="Param2" Type="String" />
                                    <asp:Parameter Name="ID" Type="Int32" />
                                </UpdateParameters>
                            </asp:SqlDataSource>
                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
                <dx:TabPage Text="시스템 리빌드">
                    <ContentCollection>
                        <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                            <dx:ASPxButton ID="ASPxButton1" runat="server" OnClick="ASPxButton1_Click" Text="자막 빌딩">
                            </dx:ASPxButton>
                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
            </TabPages>
        </dx:ASPxPageControl>
    </form>
</body>
</html>
