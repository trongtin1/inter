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
  $("#id, #email").select2({
    width: "100%",
    placeholder: "Select an option",
    allowClear: true,
  });

  $("#emailCc, #emailBcc").select2({
    width: "100%",
    placeholder: "Select options",
    allowClear: true,
    multiple: true,
  });
}

function setupEventListeners() {
  $(".select2-dropdown, #isSend, #timeType").on("change", function () {
    if (!filterHandler.isInitializing) {
      handleFilterChange();
    }
  });

  $("#sendStatus, #fromDate, #toDate").on("input", function () {
    if (!filterHandler.isInitializing) {
      handleFilterChange();
    }
  });

  $("#selectAll")
    .off("change")
    .on("change", function () {
      const isChecked = $(this).prop("checked");
      $(".mail-checkbox").prop("checked", isChecked);
    });

  $(document)
    .off("change", ".mail-checkbox")
    .on("change", ".mail-checkbox", function () {
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
  const totalCheckboxes = $(".mail-checkbox").length;
  const checkedCheckboxes = $(".mail-checkbox:checked").length;
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
    isSend: params.get("isSend"),
    timeType: params.get("timeType"),
    sendStatus: params.get("sendStatus"),
    emailCc: params.get("emailCc")?.split(";"),
    emailBcc: params.get("emailBcc")?.split(";"),
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

    const response = await mailApiService.getMails(params);
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
  const selectedIds = $(".mail-checkbox:checked")
    .map(function () {
      return parseInt($(this).val());
    })
    .get();

  if (selectedIds.length === 0) {
    alert("No mails selected!");
    return;
  }

  if (confirm(`Delete ${selectedIds.length} selected mails?`)) {
    try {
      const response = await mailApiService.deleteMultipleMails(selectedIds);
      if (response?.success) {
        await filterHandler.loadFilterOptions();
        await loadData();
      }
    } catch (error) {
      alert("Error deleting mails");
    }
  }
}

function populateFilterOptions(options) {
  const idSelect = $("#id");
  idSelect.empty().append('<option value="">All</option>');
  options.ids.forEach((id) => {
    idSelect.append(`<option value="${id}">${id}</option>`);
  });

  const emailSelect = $("#email");
  emailSelect.empty().append('<option value="">All</option>');
  options.emails.forEach((email) => {
    emailSelect.append(`<option value="${email}">${email}</option>`);
  });

  const ccSelect = $("#emailCc");
  ccSelect.empty();
  options.emailCcs.forEach((cc) => {
    ccSelect.append(`<option value="${cc}">${cc}</option>`);
  });

  const bccSelect = $("#emailBcc");
  bccSelect.empty();
  options.emailBccs.forEach((bcc) => {
    bccSelect.append(`<option value="${bcc}">${bcc}</option>`);
  });

  $(".select2-dropdown").select2({
    width: "100%",
    placeholder: "Select an option",
    allowClear: true,
  });
}
