<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="KomponenetService.Admin.Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function ImagePreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#<%=imgProduct.ClientID%>').prop('src', e.target.result)
                        .width(200)
                        .height(200);
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="mb-4" runat="server">
        <asp:Label ID="lblMsg" runat="server"></asp:Label>
    </div>

    <div class="row">
        <div class="col-sm-12 col-md-4">
            <div class="card">
                <div class="card-body">
                    <h4 class="card-title">Product</h4>
                    <hr />

                    <div class="form-body">
                        <label>Product Name*</label>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" placeholder="Enter Product Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvProductName" runat="server" ForeColor="Red" Font-Size="Small"
                                        Display="Dynamic" SetFocusOnError="true" ControlToValidate="txtProductName"
                                        ErrorMessage="Product Name is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <label>Short Description</label>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:TextBox ID="txtShortDesc" runat="server" CssClass="form-control" placeholder="Enter Short Description"></asp:TextBox>
                                    
                                </div>
                            </div>
                        </div>

                        <label>Price*</label>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="Enter Price"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPrice" runat="server" ForeColor="Red" Font-Size="Small"
                                        Display="Dynamic" SetFocusOnError="true" ControlToValidate="txtPrice"
                                        ErrorMessage="Price is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <label>Company Name</label>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:TextBox ID="txtCompanyName" runat="server" CssClass="form-control" placeholder="Enter Company Name"></asp:TextBox>

                                </div>
                            </div>
                        </div>

                        <label>Category*</label>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <!-- DropDownList for Category -->
                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control"
                                        AutoPostBack="true"
                                        DataTextField="CategoryName"
                                        DataValueField="CategoryId"
                                        OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                        AppendDataBoundItems="true">
                                        <asp:ListItem Text="Select a Category" Value="0" />
                                    </asp:DropDownList>

                                    <!-- Required Field Validator -->
                                    <asp:RequiredFieldValidator ID="rfvCategoryId" runat="server" ForeColor="Red" Font-Size="Small"
                                        Display="Dynamic" SetFocusOnError="true" ControlToValidate="ddlCategory"
                                        ErrorMessage="Category is required"></asp:RequiredFieldValidator>

                                    <!-- Hidden Field for Category ID -->
                                    <asp:HiddenField ID="hfCategoryId" runat="server" Value="0" />
                                </div>
                            </div>
                        </div>

                        <label>Stock*</label>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:TextBox ID="txtStock" runat="server" CssClass="form-control" placeholder="Enter Stock Number"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvStock" runat="server" ForeColor="Red" Font-Size="Small"
                                        Display="Dynamic" SetFocusOnError="true" ControlToValidate="txtStock"
                                        ErrorMessage="Stock Number is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <label>Long Description</label>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:TextBox ID="txtLongDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="Enter Long Description"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:CheckBox ID="cbIsActive" runat="server" Text="&nbsp; IsActive" />
                                </div>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label>Product Image</label>
                            <asp:FileUpload ID="fuProductImage" runat="server" CssClass="form-control" onchange="ImagePreview(this);" />
                        </div>
                        <div class="form-group mb-3">
                            <asp:Image ID="imgProduct" runat="server" CssClass="img-thumbnail" Width="200" Height="200" />
                        </div>

                        <div class="form-group">
                            <asp:Button ID="btnAddOrUpdate" runat="server" Text="Add" CssClass="btn btn-primary"
                                OnClick="btnAddOrUpdate_Click" ValidationGroup="Product" />
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning"
                                OnClick="btnClear_Click" CausesValidation="false" />
                            <asp:HiddenField ID="hfProductId" runat="server" Value="0" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-sm-12 col-md-8">
            <div class="card">
                <div class="card-body">
                    <h4 class="card-title">Product List</h4>
                    <hr />
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <div class="input-group">
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by product name..."></asp:TextBox>
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" CausesValidation="false">
                                    <i class="fas fa-search"></i> Search
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:Repeater ID="rProduct" runat="server" OnItemCommand="rProduct_ItemCommand">
                            <HeaderTemplate>
                                <table class="table table-hover">
                                    <thead>
                                        <tr>
                                            <th>Name</th>
                                            <th>Image</th>
                                            <th>Price</th>
                                            <th>Category</th>
                                            <th>Stock</th>
                                            <th>IsActive</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("out_productname") %></td>
                                    <td>
                                        <img width="40" src="<%# KomponenetService.Utils.getImageUrl(Eval("out_productimageurl")) %>" alt="image"/>
                                    </td>
                                    <td>$<%# Eval("out_price") %></td>
                                    <td><%# Eval("out_categoryname") %></td>
                                    <td><%# Eval("out_stock") %></td>
                                    <td>
                                        <asp:Label ID="lblIsActive" runat="server"
                                            Text='<%# Convert.ToBoolean(Eval("out_isactive")) ? "Active" : "Inactive" %>'
                                            CssClass='<%# Convert.ToBoolean(Eval("out_isactive")) ? "badge bg-success text-white" : "badge bg-danger text-white" %>'>
                                        </asp:Label>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-info" CommandName="edit"
                                            CommandArgument='<%# Eval("out_productid") %>' CausesValidation="false">
                                            <i class="fa fa-edit"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-danger" CommandName="delete"
                                            CommandArgument='<%# Eval("out_productid") %>' OnClientClick="return confirm('Do you want to delete this product?');"
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
