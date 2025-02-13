const filterHandler = {
  getFilterValues() {
    return {
      id: $("#id").val(),
      email: $("#email").val(),
      isSend: $("#isSend").val(),
      timeType: $("#timeType").val(),
      sendStatus: $("#sendStatus").val(),
      emailCc: $("#emailCc").val()?.join(";"),
      emailBcc: $("#emailBcc").val()?.join(";"),
      fromDate: $("#fromDate").val(),
      toDate: $("#toDate").val(),
    };
  },

  setFilterValues(filters) {
    if (!filters) return;
    $("#id").val(filters.id).trigger("change");
    $("#email").val(filters.email).trigger("change");
    $("#isSend").val(filters.isSend);
    $("#timeType").val(filters.timeType);
    $("#sendStatus").val(filters.sendStatus);
    $("#emailCc").val(filters.emailCc?.split(";")).trigger("change");
    $("#emailBcc").val(filters.emailBcc?.split(";")).trigger("change");
    $("#fromDate").val(filters.fromDate);
    $("#toDate").val(filters.toDate);
  },

  resetFilters() {
    $("select").val("").trigger("change.select2");
    $("input").val("");
    currentPage = 1;
    loadData();
  },

  async loadFilterOptions() {
    try {
      const response = await mailApiService.getFilterOptions();
      if (response?.success) {
        populateFilterOptions(response.data);
      } else {
        console.error("Failed to load filter options:", response?.message);
      }
    } catch (error) {
      console.error("Error loading filter options:", error);
      alert("Unable to load filter options. Please try refreshing the page.");
    }
  },
};
