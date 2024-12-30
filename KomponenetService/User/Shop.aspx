<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs" Inherits="KomponenetService.User.Shop" EnableEventValidation="false" %>
<%@ Import Namespace="KomponenetService" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Shop Start -->
    <div class="container-fluid pt-5">
        <div class="row px-xl-5">
            <!-- Shop Sidebar Start -->
            <div class="col-lg-3 col-md-12">
                <!-- Categories Start -->
                <div class="border-bottom mb-4 pb-4">
                    <h5 class="font-weight-semi-bold mb-4">Filter by Categories</h5>
                    <asp:Repeater ID="rCategories" runat="server" OnItemCommand="rCategories_ItemCommand">
                        <ItemTemplate>
                            <div class="custom-control custom-checkbox d-flex align-items-center justify-content-between mb-3">
                                <asp:LinkButton ID="lbCategory" runat="server" CssClass="text-dark" 
                                    CommandName="category" CommandArgument='<%# Eval("CategoryId") %>'>
                                    <%# Eval("CategoryName") %> <small class="text-muted">(<%# Eval("ProductCount") %>)</small>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <!-- Categories End -->

                <!-- Price Start -->
                <div class="border-bottom mb-4 pb-4">
                    <h5 class="font-weight-semi-bold mb-4">Filter by price</h5>
                    <div class="custom-control custom-checkbox d-flex align-items-center justify-content-between mb-3">
                        <asp:RadioButton ID="rbAll" runat="server" GroupName="price" Text="All Price" Checked="true" AutoPostBack="true" OnCheckedChanged="PriceFilter_CheckedChanged" />
                    </div>
                    <div class="custom-control custom-checkbox d-flex align-items-center justify-content-between mb-3">
                        <asp:RadioButton ID="rb0_100" runat="server" GroupName="price" Text="$0 - $100" AutoPostBack="true" OnCheckedChanged="PriceFilter_CheckedChanged" />
                    </div>
                    <div class="custom-control custom-checkbox d-flex align-items-center justify-content-between mb-3">
                        <asp:RadioButton ID="rb100_500" runat="server" GroupName="price" Text="$100 - $500" AutoPostBack="true" OnCheckedChanged="PriceFilter_CheckedChanged" />
                    </div>
                    <div class="custom-control custom-checkbox d-flex align-items-center justify-content-between mb-3">
                        <asp:RadioButton ID="rb500plus" runat="server" GroupName="price" Text="$500+" AutoPostBack="true" OnCheckedChanged="PriceFilter_CheckedChanged" />
                    </div>
                </div>
                <!-- Price End -->
            </div>
            <!-- Shop Sidebar End -->

            <!-- Shop Product Start -->
            <div class="col-lg-9">
                <div class="row pb-3">
                    <div class="col-12 pb-1">
                        <div class="d-flex align-items-center justify-content-between mb-4">
                            <div class="input-group">
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by name"></asp:TextBox>
                                <div class="input-group-append">
                                    <asp:LinkButton ID="lbSearch" runat="server" CssClass="input-group-text bg-transparent text-primary" OnClick="lbSearch_Click">
                                        <i class="fa fa-search"></i>
                                    </asp:LinkButton>
                                </div>
                            </div>
                            <div class="dropdown ml-4">
                                <asp:DropDownList ID="ddlSort" runat="server" CssClass="btn border dropdown-toggle" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
                                    <asp:ListItem Text="Sort by" Value="0" />
                                    <asp:ListItem Text="Latest" Value="latest" />
                                    <asp:ListItem Text="Price: Low to High" Value="price_asc" />
                                    <asp:ListItem Text="Price: High to Low" Value="price_desc" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <asp:Repeater ID="rProducts" runat="server" OnItemCommand="rProducts_ItemCommand">
                        <ItemTemplate>
                            <div class="col-lg-4 col-md-6 col-sm-12 pb-1">
                                <div class="card product-item border-0 mb-4">
                                    <div class="card-header product-img position-relative overflow-hidden bg-transparent border p-0">
                                        <img class="img-fluid w-100" src='<%# Utils.getImageUrl(Eval("ProductImageUrl")) %>' alt="">
                                    </div>
                                    <div class="card-body border-left border-right text-center p-0 pt-4 pb-3">
                                        <h6 class="text-truncate mb-3"><%# Eval("ProductName") %></h6>
                                        <p class="text-muted mb-3" style="height: 50px; overflow: hidden;"><%# Eval("ShortDescription") %></p>
                                        <div class="d-flex justify-content-center">
                                            <h6>$<%# Eval("Price") %></h6>
                                        </div>
                                    </div>
                                    <div class="card-footer d-flex justify-content-between bg-light border">
                                        <asp:LinkButton ID="lbViewDetail" runat="server" CssClass="btn btn-sm text-dark p-0" 
                                            CommandName="viewDetail" CommandArgument='<%# Eval("ProductId") %>'>
                                            <i class="fas fa-eye text-primary mr-1"></i>View Detail
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lbAddToCart" runat="server" CssClass="btn btn-sm text-dark p-0" 
                                            CommandName="addToCart" CommandArgument='<%# Eval("ProductId") %>'>
                                            <i class="fas fa-shopping-cart text-primary mr-1"></i>Add To Cart
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <!-- Pagination Start -->
                    <div class="col-12 pb-1">
                        <nav aria-label="Page navigation">
                            <ul class="pagination justify-content-center mb-3">
                                <li class="page-item">
                                    <asp:LinkButton ID="lbPrevious" runat="server" CssClass="page-link" aria-label="Previous" OnClick="lbPrevious_Click">
                                        <span aria-hidden="true">&laquo;</span>
                                        <span class="sr-only">Previous</span>
                                    </asp:LinkButton>
                                </li>
                                <asp:Repeater ID="rPaging" runat="server" OnItemCommand="rPaging_ItemCommand">
                                    <ItemTemplate>
                                        <li class="page-item <%# Container.DataItem.ToString() == ViewState["CurrentPage"].ToString() ? "active" : "" %>">
                                            <asp:LinkButton ID="lbPaging" runat="server" CssClass="page-link" 
                                                CommandName="page" CommandArgument='<%# Container.DataItem %>'>
                                                <%# Container.DataItem %>
                                            </asp:LinkButton>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <li class="page-item">
                                    <asp:LinkButton ID="lbNext" runat="server" CssClass="page-link" aria-label="Next" OnClick="lbNext_Click">
                                        <span aria-hidden="true">&raquo;</span>
                                        <span class="sr-only">Next</span>
                                    </asp:LinkButton>
                                </li>
                            </ul>
                        </nav>
                    </div>
                    <!-- Pagination End -->
                </div>
            </div>
            <!-- Shop Product End -->
        </div>
    </div>
    <!-- Shop End -->

</asp:Content>
