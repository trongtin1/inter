let currentPage = 1;
let totalPages = 1;

$(document).ready(function () {
  initializeSelect2();
  filterHandler.loadFilterOptions().then(() => {
    if (!filterHandler.loadFiltersFromStorage()) {
      loadFiltersFromUrl();
    }
    loadData();
  });
  setupEventListeners();
});

function initializeSelect2() {
  $("#id, #email, #type").select2({
    width: "100%",
    placeholder: "Select an option",
    allowClear: true,
  });
}

function setupEventListeners() {
  $(".select2-dropdown, #isRead, #isSeen, #timeType, #type").on(
    "change",
    function () {
      if (!filterHandler.isInitializing) {
        handleFilterChange();
      }
    }
  );

  $("#from, #fromDate, #toDate").on("input", function () {
    if (!filterHandler.isInitializing) {
      handleFilterChange();
    }
  });

  $("#selectAll")
    .off("change")
    .on("change", function () {
      const isChecked = $(this).prop("checked");
      $(".notification-checkbox").prop("checked", isChecked);
    });

  $(document)
    .off("change", ".notification-checkbox")
    .on("change", ".notification-checkbox", function () {
      updateSelectAllState();
    });
}

function handleFilterChange() {
  currentPage = 1;
  loadData();
  updateUrlWithFilters();
  filterHandler.saveFiltersToStorage();
}

function updateSelectAllState() {
  const totalCheckboxes = $(".notification-checkbox").length;
  const checkedCheckboxes = $(".notification-checkbox:checked").length;
  $("#selectAll").prop(
    "checked",
    totalCheckboxes > 0 && totalCheckboxes === checkedCheckboxes
  );
}

function updateUrlWithFilters() {
  const params = new URLSearchParams();

  if (currentPage > 1) {
    params.append("page", currentPage);
  }

  const filters = filterHandler.getFilterValues();
  Object.entries(filters).forEach(([key, value]) => {
    if (value) {
      params.append(key, value);
    }
  });

  const queryString = params.toString();
  const newUrl = queryString
    ? `${window.location.pathname}?${queryString}`
    : window.location.pathname;

  window.history.pushState({}, "", newUrl);
}

function loadFiltersFromUrl() {
  const params = new URLSearchParams(window.location.search);

  const filters = {
    id: params.get("id"),
    email: params.get("email"),
    type: params.get("type"),
    isRead: params.get("isRead"),
    isSeen: params.get("isSeen"),
    timeType: params.get("timeType"),
    fromDate: params.get("fromDate"),
    toDate: params.get("toDate"),
  };

  filterHandler.setFilterValues(filters);
  currentPage = parseInt(params.get("page")) || 1;
}

async function loadData() {
  try {
    const filters = filterHandler.getFilterValues();
    const params = new URLSearchParams({
      page: currentPage,
      ...filters,
    });

    updateUrlWithFilters();

    const response = await notificationApiService.getNotifications(params);
    if (response?.success) {
      const paginatedData = response.data;
      renderHandler.renderTable(paginatedData.items);
      renderHandler.renderPagination({
        pageIndex: paginatedData.pageIndex,
        totalPages: paginatedData.totalPages,
        hasPreviousPage: paginatedData.hasPreviousPage,
        hasNextPage: paginatedData.hasNextPage,
      });
    }
  } catch (error) {
    console.error("Error loading data:", error);
  }
}

function changePage(page) {
  if (page >= 1 && page <= totalPages) {
    event.preventDefault();
    const currentScroll = window.scrollY;
    currentPage = page;
    loadData().then(() => {
      window.scrollTo(0, currentScroll);
    });
    updateUrlWithFilters();
  }
}

async function deleteSelected() {
  const selectedIds = $(".notification-checkbox:checked")
    .map(function () {
      return parseInt($(this).val());
    })
    .get();

  if (selectedIds.length === 0) {
    alert("No notifications selected!");
    return;
  }

  if (confirm(`Delete ${selectedIds.length} selected notifications?`)) {
    try {
      const response = await notificationApiService.deleteMultipleNotifications(
        selectedIds
      );
      if (response?.success) {
        await filterHandler.loadFilterOptions();
        await loadData();
      }
    } catch (error) {
      alert("Error deleting notifications");
    }
  }
}

function populateFilterOptions(options) {
  // Populate ID select
  const idSelect = $("#id");
  idSelect.empty().append('<option value="">All</option>');
  options.ids.forEach((id) => {
    idSelect.append(`<option value="${id}">${id}</option>`);
  });

  // Populate email select with full profile information (same as from)
  const emailSelect = $("#email");
  emailSelect.empty().append('<option value="">All</option>');
  options.profiles.forEach((profile) => {
    emailSelect.append(
      `<option value="${profile.email}">${profile.fullName} (${profile.email})</option>`
    );
  });

  // Populate from select with full profile information
  const fromSelect = $("#from");
  fromSelect.empty().append('<option value="">All</option>');
  options.profiles.forEach((profile) => {
    fromSelect.append(
      `<option value="${profile.fullName}">${profile.fullName} (${profile.email})</option>`
    );
  });
  // Populate type select
  const typeSelect = $("#type");
  typeSelect.empty().append('<option value="">All</option>');
  options.types.forEach((type) => {
    typeSelect.append(`<option value="${type}">${type}</option>`);
  });

  // Apply Select2 to dropdowns
  $("#id, #email, #from, #type").select2({
    width: "100%",
    placeholder: "Select an option",
    allowClear: true,
    matcher: function (params, data) {
      if (!params.term) {
        return data;
      }
      const searchTerm = params.term.toLowerCase();
      const text = data.text.toLowerCase();
      if (text.includes(searchTerm)) {
        return data;
      }
      return null;
    },
  });
}
