const filterHandler = {
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
    $("#id").val(filters.id);
    $("#email").val(filters.email);
    $("#from").val(filters.from);
    $("#type").val(filters.type);
    $("#isRead").val(filters.isRead);
    $("#isSeen").val(filters.isSeen);
    $("#timeType").val(filters.timeType);
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
};
