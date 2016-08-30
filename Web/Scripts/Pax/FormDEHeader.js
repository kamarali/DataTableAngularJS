function InitializeEditFormDE() {
  $("#BilledMemberText").attr("readonly", "readonly");
  $("#SettlementMethodId").attr("disabled", "disabled");
  $("#InvoiceDate").attr("readonly", "readonly");

  $("#FormDEProvisionalBillingMonth").attr("disabled", "disabled");
  $("#ListingCurrencyId").attr("disabled", "disabled");
  $("#BillingCurrencyId").attr("disabled", "disabled");
  $("#ListingToBillingRate").attr("readonly", true);
}

function setFocusForFormDEHeader() {
  $('#FormDEProvisionalBillingMonth').focus();
}