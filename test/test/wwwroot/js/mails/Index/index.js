let currentPage = 1;
let totalPages = 1;

$(document).ready(function () {
  initializeSelect2();
  loadFiltersFromUrl();
  sessionStorage.removeItem("filterOptions");
  filterHandler.loadFilterOptions().then(() => {
    filterHandler.loadSavedFilters();
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
  // Dropdown filters (select elements)
  $(".select2-dropdown, #isSend, #timeType").on("change", function () {
    handleFilterChange();
  });

  // Input filters (text and datetime inputs)
  $("#sendStatus, #fromDate, #toDate").on("input", function () {
    handleFilterChange();
  });

  // Handle select all checkbox
  $("#selectAll")
    .off("change")
    .on("change", function () {
      const isChecked = $(this).prop("checked");
      $(".mail-checkbox").prop("checked", isChecked);
    });

  // Handle individual checkboxes
  $(document)
    .off("change", ".mail-checkbox")
    .on("change", ".mail-checkbox", function () {
      updateSelectAllState();
    });
}

function handleFilterChange() {
  currentPage = 1; // Đặt lại về trang đầu tiên
  loadData(); // Gọi API ngay lập tức
  updateUrlWithFilters(); // Cập nhật URL với bộ lọc mới
  filterHandler.saveFilters(); // Lưu bộ lọc vào sessionStorage
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

  // Chỉ thêm các filter có giá trị hợp lệ
  Object.entries(filters).forEach(([key, value]) => {
    if (value) {
      if (Array.isArray(value)) {
        // Xử lý mảng (cho emailCc và emailBcc)
        const validValues = value.filter((v) => v && v.trim() !== "");
        if (validValues.length > 0) {
          params.append(key, validValues.join(";"));
        }
      } else if (typeof value === "string" && value.trim() !== "") {
        // Xử lý chuỗi
        params.append(key, value.trim());
      } else if (typeof value === "boolean" || typeof value === "number") {
        // Xử lý boolean và number
        params.append(key, value.toString());
      }
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
    // Prevent default anchor behavior
    event.preventDefault();

    // Store current scroll position
    const currentScroll = window.scrollY;

    currentPage = page;
    loadData().then(() => {
      // Restore scroll position after data loads
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
        // Clear cache to force reload of filter options
        sessionStorage.removeItem("filterOptions");
        // Reload filter options and data
        await filterHandler.loadFilterOptions();
        await loadData();
      }
    } catch (error) {
      alert("Error deleting mails");
    }
  }
}

function populateFilterOptions(options) {
  // Populate ID dropdown
  const idSelect = $("#id");
  idSelect.empty().append('<option value="">All</option>');
  options.ids.forEach((id) => {
    idSelect.append(`<option value="${id}">${id}</option>`);
  });

  // Populate Email dropdown
  const emailSelect = $("#email");
  emailSelect.empty().append('<option value="">All</option>');
  options.emails.forEach((email) => {
    emailSelect.append(`<option value="${email}">${email}</option>`);
  });

  // Populate CC dropdown
  const ccSelect = $("#emailCc");
  ccSelect.empty();
  options.emailCcs.forEach((cc) => {
    ccSelect.append(`<option value="${cc}">${cc}</option>`);
  });

  // Populate BCC dropdown
  const bccSelect = $("#emailBcc");
  bccSelect.empty();
  options.emailBccs.forEach((bcc) => {
    bccSelect.append(`<option value="${bcc}">${bcc}</option>`);
  });

  // Reinitialize Select2
  $(".select2-dropdown").select2({
    width: "100%",
    placeholder: "Select an option",
    allowClear: true,
  });
}
