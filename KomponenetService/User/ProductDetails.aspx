<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="KomponenetService.User.ProductDetails" %>
<%@ Import Namespace="KomponenetService" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Shop Detail Start -->
    <div class="container-fluid py-5">
        <div class="row px-xl-5">
            <div class="col-lg-5 pb-5">
                <div id="product-carousel" class="carousel slide" data-ride="carousel">
                    <div class="carousel-inner border">
                        <div class="carousel-item active">
                            <asp:Image ID="imgProduct" runat="server" CssClass="w-100 h-100" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-7 pb-5">
                <h3 class="font-weight-semi-bold"><asp:Label ID="lblName" runat="server"></asp:Label></h3>
                <div class="d-flex mb-3">
                    <div class="text-primary mr-2">
                        <small class="fas fa-star"></small>
                        <small class="fas fa-star"></small>
                        <small class="fas fa-star"></small>
                        <small class="fas fa-star-half-alt"></small>
                        <small class="far fa-star"></small>
                    </div>
                    <small class="pt-1">(50 Reviews)</small>
                </div>
                <h3 class="font-weight-semi-bold mb-4">$<asp:Label ID="lblPrice" runat="server"></asp:Label></h3>
                <p class="mb-4"><asp:Label ID="lblDescription" runat="server"></asp:Label></p>
                <div class="d-flex align-items-center mb-4 pt-2">
                    <div class="input-group quantity mr-3" style="width: 130px;">
                        <div class="input-group-btn">
                            <button class="btn btn-primary btn-minus" type="button">
                                <i class="fa fa-minus"></i>
                            </button>
                        </div>
                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control bg-secondary text-center" Text="1"></asp:TextBox>
                        <div class="input-group-btn">
                            <button class="btn btn-primary btn-plus" type="button">
                                <i class="fa fa-plus"></i>
                            </button>
                        </div>
                    </div>
                    <asp:Button ID="btnAddToCart" runat="server" Text="Add To Cart" CssClass="btn btn-primary px-3"
                        OnClick="btnAddToCart_Click" />
                </div>
                <div class="d-flex pt-2">
                    <p class="text-dark font-weight-medium mb-0 mr-2">Share on:</p>
                    <div class="d-inline-flex">
                        <a class="text-dark px-2" href="">
                            <i class="fab fa-facebook-f"></i>
                        </a>
                        <a class="text-dark px-2" href="">
                            <i class="fab fa-twitter"></i>
                        </a>
                        <a class="text-dark px-2" href="">
                            <i class="fab fa-linkedin-in"></i>
                        </a>
                        <a class="text-dark px-2" href="">
                            <i class="fab fa-pinterest"></i>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Shop Detail End -->

    <!-- Products Start -->
    <div class="container-fluid py-5">
        <div class="text-center mb-4">
            <h2 class="section-title px-5"><span class="px-2">You May Also Like</span></h2>
        </div>
        <div class="row px-xl-5">
            <div class="col">
                <div class="owl-carousel related-carousel">
                    <asp:Repeater ID="rRelatedProducts" runat="server" OnItemCommand="rRelatedProducts_ItemCommand">
                        <ItemTemplate>
                            <div class="card product-item border-0">
                                <div class="card-header product-img position-relative overflow-hidden bg-transparent border p-0">
                                    <img class="img-fluid w-100" src='<%# Utils.getImageUrl(Eval("ImageUrl")) %>' alt="">
                                </div>
                                <div class="card-body border-left border-right text-center p-0 pt-4 pb-3">
                                    <h6 class="text-truncate mb-3"><%# Eval("Name") %></h6>
                                    <div class="d-flex justify-content-center">
                                        <h6>$<%# Eval("Price") %></h6>
                                    </div>
                                </div>
                                <div class="card-footer d-flex justify-content-between bg-light border">
                                    <a href='ProductDetails.aspx?id=<%# Eval("ProductId") %>' class="btn btn-sm text-dark p-0">
                                        <i class="fas fa-eye text-primary mr-1"></i>View Detail
                                    </a>
                                    <asp:LinkButton ID="lbAddToCart" runat="server" CssClass="btn btn-sm text-dark p-0"
                                        CommandName="addToCart" CommandArgument='<%# Eval("ProductId") %>'>
                                        <i class="fas fa-shopping-cart text-primary mr-1"></i>Add To Cart
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
    <!-- Products End -->

</asp:Content> 