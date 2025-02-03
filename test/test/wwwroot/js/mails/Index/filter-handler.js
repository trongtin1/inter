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
  saveFilters() {
    const filters = this.getFilterValues();
    sessionStorage.setItem("mailFilters", JSON.stringify(filters));
  },

  loadSavedFilters() {
    const savedFilters = sessionStorage.getItem("mailFilters");
    if (savedFilters) {
      this.setFilterValues(JSON.parse(savedFilters));
    }
  },

  resetFilters() {
    $("select").val("").trigger("change.select2");
    $("input").val("");
    currentPage = 1;
    sessionStorage.removeItem("mailFilters"); // Clear saved filters
    loadData();
  },
  async loadFilterOptions() {
    try {
      const options = await mailApiService.getFilterOptions();
      if (options) {
        populateFilterOptions(options);
      }
    } catch (error) {
      console.error("Error loading filter options:", error);
    }
  },
};
