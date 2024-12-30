<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="KomponenetService.User.Cart" %>
<%@ Import Namespace="KomponenetService" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(document).ready(function () {
            // Miktar artırma/azaltma butonları için
            $('.btn-plus').click(function (e) {
                e.preventDefault();
                var quantityInput = $(this).closest('.quantity').find('input');
                var currentVal = parseInt(quantityInput.val());
                quantityInput.val(currentVal + 1);
            });

            $('.btn-minus').click(function (e) {
                e.preventDefault();
                var quantityInput = $(this).closest('.quantity').find('input');
                var currentVal = parseInt(quantityInput.val());
                if (currentVal > 1) {
                    quantityInput.val(currentVal - 1);
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Cart Start -->
    <div class="container-fluid pt-5">
        <div class="row px-xl-5">
            <div class="col-lg-8 table-responsive mb-5">
                <asp:Repeater ID="rCartItems" runat="server" OnItemCommand="rCartItems_ItemCommand">
                    <HeaderTemplate>
                        <table class="table table-bordered text-center mb-0">
                            <thead class="bg-secondary text-dark">
                                <tr>
                                    <th>Product</th>
                                    <th>Price</th>
                                    <th>Quantity</th>
                                    <th>Total</th>
                                    <th>Remove</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="align-middle">
                                <img src='<%# Utils.getImageUrl(Eval("imageurl")) %>' alt="" style="width: 50px;">
                                <%# Eval("name") %>
                            </td>
                            <td class="align-middle">$<%# Eval("price") %></td>
                            <td class="align-middle">
                                <div class="input-group quantity mx-auto" style="width: 100px;">
                                    <div class="input-group-btn">
                                        <button class="btn btn-sm btn-primary btn-minus">
                                            <i class="fa fa-minus"></i>
                                        </button>
                                    </div>
                                    <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Eval("quantity") %>'
                                        CssClass="form-control form-control-sm bg-secondary text-center"
                                        CommandArgument='<%# Eval("productid") %>' />
                                    <div class="input-group-btn">
                                        <button class="btn btn-sm btn-primary btn-plus">
                                            <i class="fa fa-plus"></i>
                                        </button>
                                    </div>
                                </div>
                            </td>
                            <td class="align-middle">$<%# Convert.ToDecimal(Eval("price")) * Convert.ToInt32(Eval("quantity")) %></td>
                            <td class="align-middle">
                                <asp:LinkButton ID="lbDelete" runat="server" CssClass="btn btn-sm btn-primary"
                                    CommandName="remove" CommandArgument='<%# Eval("productid") %>'>
                                    <i class="fa fa-times"></i>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <div class="text-right mt-3">
                    <asp:Button ID="btnUpdateCart" runat="server" Text="Update Cart" 
                        CssClass="btn btn-primary" OnClick="btnUpdateCart_Click" />
                </div>
            </div>
            <div class="col-lg-4">
                <div class="card border-secondary mb-5">
                    <div class="card-header bg-secondary border-0">
                        <h4 class="font-weight-semi-bold m-0">Summary</h4>
                    </div>
                    <div class="card-body">
                        <div class="d-flex justify-content-between mb-3 pt-1">
                            <h6 class="font-weight-medium">Subtotal</h6>
                            <h6 class="font-weight-medium">$<asp:Label ID="lblSubTotal" runat="server" /></h6>
                        </div>
                        <div class="d-flex justify-content-between">
                            <h6 class="font-weight-medium">Cargo</h6>
                            <h6 class="font-weight-medium">$<asp:Label ID="lblShipping" runat="server" /></h6>
                        </div>
                    </div>
                    <div class="card-footer border-secondary bg-transparent">
                        <div class="d-flex justify-content-between mt-2">
                            <h5 class="font-weight-bold">Total</h5>
                            <h5 class="font-weight-bold">$<asp:Label ID="lblTotal" runat="server" /></h5>
                        </div>
                        <asp:LinkButton ID="lbCheckout" runat="server" CssClass="btn btn-block btn-primary my-3 py-3"
                            OnClick="lbCheckout_Click">
                            Complete The Order
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Cart End -->

</asp:Content> 