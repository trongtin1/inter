const filterHandler = {
  isInitializing: false,

  getFilterValues() {
    return {
      id: $("#id").val(),
      email: $("#email").val(),
      from: $("#from").val(),
      type: $("#type").val(),
      isRead: $("#isRead").val(),
      isSeen: $("#isSeen").val(),
      timeType: $("#timeType").val(),
      fromDate: $("#fromDate").val(),
      toDate: $("#toDate").val(),
    };
  },

  setFilterValues(filters) {
    if (!filters) return;

    this.isInitializing = true;

    $("#id").val(filters.id).trigger("change", [true]);
    $("#email").val(filters.email).trigger("change", [true]);
    $("#from").val(filters.from).trigger("change", [true]);
    $("#type").val(filters.type).trigger("change", [true]);
    $("#isRead").val(filters.isRead);
    $("#isSeen").val(filters.isSeen);
    $("#timeType").val(filters.timeType);
    $("#fromDate").val(filters.fromDate);
    $("#toDate").val(filters.toDate);

    this.isInitializing = false;

    this.saveFiltersToStorage();
  },

  resetFilters() {
    this.isInitializing = true;
    $("select").val("").trigger("change.select2");
    $("input").val("");
    currentPage = 1;
    sessionStorage.removeItem("notificationFilters");
    this.isInitializing = false;
    loadData();
  },

  async loadFilterOptions() {
    try {
      const response = await notificationApiService.getFilterOptions();
      if (response?.success) {
        populateFilterOptions(response.data);
      } else {
        console.error("Failed to load filter options:", response?.message);
      }
    } catch (error) {
      console.error("Error loading filter options:", error);
    }
  },

  saveFiltersToStorage() {
    const filters = this.getFilterValues();
    sessionStorage.setItem(
      "notificationFilters",
      JSON.stringify({
        ...filters,
        page: currentPage,
      })
    );
  },

  loadFiltersFromStorage() {
    const savedFilters = sessionStorage.getItem("notificationFilters");
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
