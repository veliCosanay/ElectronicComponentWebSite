<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Categories.aspx.cs" Inherits="KomponenetService.Admin.Categories" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
        window.onload = function () {
            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%=lblMsg.ClientID%>").style.display = "none";
            }, seconds * 1000);
        };
    </script>

    <script>
        function ImagePreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#<%=imagePreview.ClientID%>').prop('src', e.target.result).width(200).height(200);
                };
                reader.readAsDataURL(input.files[0])
            }
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="mb-4">
        <asp:Label ID="lblMsg" runat="server"></asp:Label>
    </div>

    <div class="row">
    <div class="col-sm-12 col-md-4">
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Category</h4>
                <hr />

                <div class="form-body">

                    <label>Category Name</label>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control" placeholder="Enter Category Name"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCategoryName" runat="server" ForeColor="Red" Font-Size="Small"
                                    Display="Dynamic" SetFocusOnError="true" ControlToValidate="txtCategoryName"
                                    ErrorMessage="Category Name is required"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>

                    <label>Category Image</label>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:FileUpload ID="fuCategoryImage" runat="server" CssClass="form-control" 
                                    onchange="ImagePreview(this); "/>
                                <asp:HiddenField ID="hfCategoryId" runat="server" Value="0"/>
                                
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:CheckBox ID="cbIsActive" runat="server" Text="&nbsp; IsActive"/>
                            </div>
                        </div>
                    </div>

                    <div class="form-actiona pb-5">
                        <div class="text-left">
                            <asp:Button ID="btnAddOrUpdate" runat="server" CssClass="btn btn-info" Text="Add" OnClick="btnAddOrUpdate_Click"/>
                            <asp:Button ID="btnClear" runat="server" CssClass="btn btn-dark" Text="Reset" OnClick="btnClear_Click"/>
                        </div>
                    </div>

                    <div>
                        <asp:Image ID="imagePreview" runat="server" CssClass="img-thumbnail" AlternateText=""/>
                    </div>

                </div>

            </div>
        </div>
    </div>

    <div class="col-sm-12 col-md-8">
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Category List</h4>
                <hr />
                <div class="table-responsive">
                    <asp:Repeater ID="rCategory" runat="server" OnItemCommand="rCategory_ItemCommand">
                        <HeaderTemplate>
                            <table class="table">
                                <thead class="bg-light">
                                    <tr>
                                        <th>Name</th>
                                        <th>Image</th>
                                        <th>IsActive</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("out_CategoryName") %></td>
                                <td>
                                    <img width="40" src="<%# KomponenetService.Utils.getImageUrl(Eval("out_CategoryImageUrl")) %>" alt="image"/>
                                </td>
                                <td>
                                    <asp:Label ID="lblIsActive" runat="server"
                                        Text='<%# Convert.ToBoolean(Eval("out_IsActive")) ? "Active" : "Inactive" %>'
                                        CssClass='<%# Convert.ToBoolean(Eval("out_IsActive")) ? "badge bg-success text-white" : "badge bg-danger text-white" %>'>
                                    </asp:Label>
                                </td>
                                <td>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-info" CommandName="edit"
                                        CommandArgument='<%# Eval("out_CategoryId") %>' CausesValidation="false">
                                        <i class="fa fa-edit"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-danger" CommandName="delete"
                                        CommandArgument='<%# Eval("out_CategoryId") %>' OnClientClick="return confirm('Do you want to delete this category?');"
                                        CausesValidation="false">
                                        <i class="fa fa-trash"></i>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

</div>

</asp:Content>
