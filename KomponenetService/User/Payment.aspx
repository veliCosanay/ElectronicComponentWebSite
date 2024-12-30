<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="KomponenetService.User.Payment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid pt-5">
        <div class="row px-xl-5">
            <div class="col-lg-8">
                <div class="mb-4">
                    <h4 class="font-weight-semi-bold mb-4">Payment</h4>
                    <div class="row">
                        <div class="col-md-6 form-group">
                            <label>Name on Card</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" required="required"></asp:TextBox>
                        </div>
                        <div class="col-md-6 form-group">
                            <label>Card Number</label>
                            <asp:TextBox ID="txtCardNo" runat="server" CssClass="form-control" MaxLength="16" required="required"></asp:TextBox>
                        </div>
                        <div class="col-md-6 form-group">
                            <label>Expiry Date (MM/YY)</label>
                            <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" placeholder="MM/YY" MaxLength="5" required="required"></asp:TextBox>
                        </div>
                        <div class="col-md-6 form-group">
                            <label>CVV</label>
                            <asp:TextBox ID="txtCvv" runat="server" CssClass="form-control" MaxLength="3" TextMode="Password" required="required"></asp:TextBox>
                        </div>
                        <div class="col-md-12 form-group">
                            <label>Address</label>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" required="required"></asp:TextBox>
                        </div>
                        <div class="col-md-12 form-group">
                            <label>Payment Mode</label>
                            <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Credit Card" Value="Credit Card" />
                                <asp:ListItem Text="Debit Card" Value="Debit Card" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-4">
                <div class="card border-secondary mb-5">
                    <div class="card-header bg-secondary border-0">
                        <h4 class="font-weight-semi-bold m-0">Order Total</h4>
                    </div>
                    <div class="card-body">
                        <div class="d-flex justify-content-between mb-3 pt-1">
                            <h6 class="font-weight-medium">Subtotal</h6>
                            <h6 class="font-weight-medium">$<asp:Label ID="lblSubTotal" runat="server" /></h6>
                        </div>
                        <div class="d-flex justify-content-between">
                            <h6 class="font-weight-medium">Shipping</h6>
                            <h6 class="font-weight-medium">$<asp:Label ID="lblShipping" runat="server" /></h6>
                        </div>
                    </div>
                    <div class="card-footer border-secondary bg-transparent">
                        <div class="d-flex justify-content-between mt-2">
                            <h5 class="font-weight-bold">Total</h5>
                            <h5 class="font-weight-bold">$<asp:Label ID="lblTotal" runat="server" /></h5>
                        </div>
                        <asp:Button ID="btnPayNow" runat="server" Text="Pay Now" 
                            CssClass="btn btn-lg btn-block btn-primary font-weight-bold my-3 py-3"
                            OnClick="btnPayNow_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content> 