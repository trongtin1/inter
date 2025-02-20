const filterHandler = {
  isInitializing: false,

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

    this.isInitializing = true;

    $("#id").val(filters.id).trigger("change", [true]);
    $("#email").val(filters.email).trigger("change", [true]);
    $("#isSend").val(filters.isSend);
    $("#timeType").val(filters.timeType);
    $("#sendStatus").val(filters.sendStatus);
    $("#emailCc").val(filters.emailCc?.split(";")).trigger("change", [true]);
    $("#emailBcc").val(filters.emailBcc?.split(";")).trigger("change", [true]);
    $("#fromDate").val(filters.fromDate);
    $("#toDate").val(filters.toDate);

    this.isInitializing = false;

    this.saveFiltersToStorage();
  },

  resetFilters() {
    $("select").val("").trigger("change.select2");
    $("input").val("");
    currentPage = 1;
    localStorage.removeItem("mailFilters");
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

  saveFiltersToStorage() {
    const filters = this.getFilterValues();
    localStorage.setItem(
      "mailFilters",
      JSON.stringify({
        ...filters,
        page: currentPage,
      })
    );
  },

  loadFiltersFromStorage() {
    const savedFilters = localStorage.getItem("mailFilters");
    if (savedFilters) {
      const filters = JSON.parse(savedFilters);
      currentPage = filters.page || 1;
      delete filters.page;
      this.setFilterValues(filters);
      return true;
    }
    return false;
  },
};
