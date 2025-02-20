async function exportFile() {
  try {
    const defaultFileName = `Mails_Export_${new Date()
      .toISOString()
      .slice(0, 10)}.xlsx`;

    // Get current filters from filterHandler
    const filters = filterHandler.getFilterValues();

    // Build query parameters
    const params = new URLSearchParams({
      fileName: defaultFileName,
      ...filters,
    });

    const response = await axios.get(
      `${mailApiService.baseUrl}/mail-excel/export?${params}`,
      {
        headers: {
          Authorization: `Bearer ${mailApiService.getToken()}`,
        },
        responseType: "blob",
      }
    );

    // Check if the response has the success header
    const isSuccess = response.headers["x-success"] === "true";
    if (!isSuccess) {
      throw new Error(response.headers["x-message"] || "Export failed");
    }

    const blob = new Blob([response.data], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });

    if (window.showSaveFilePicker) {
      try {
        const handle = await window.showSaveFilePicker({
          suggestedName: defaultFileName,
          types: [
            {
              description: "Excel Files",
              accept: {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                  [".xlsx"],
              },
            },
          ],
        });
        const writable = await handle.createWritable();
        await writable.write(blob);
        await writable.close();
      } catch (err) {
        if (err.name === "AbortError") return;
        throw err;
      }
    } else {
      const url = window.URL.createObjectURL(blob);
      const $a = $("<a>", {
        href: url,
        download: defaultFileName,
      });
      $("body").append($a);
      $a[0].click(); // Sử dụng jQuery để click
      $a.remove(); // Xóa phần tử sau khi click
      window.URL.revokeObjectURL(url);
    }
  } catch (error) {
    console.error("Export error:", error);
    if (error.name !== "AbortError") {
      alert(
        "Error exporting file: " +
          (error.response?.data?.message || error.message)
      );
    }
  }
}

async function importFile() {
  // Tạo input file ẩn bằng jQuery
  const $fileInput = $("<input>", {
    type: "file",
    accept: ".xlsx",
  });

  // Gán sự kiện onchange
  $fileInput.on("change", async function (e) {
    try {
      const file = e.target.files[0];
      if (!file) return;

      // Kiểm tra định dạng file
      if (!file.name.toLowerCase().endsWith(".xlsx")) {
        alert("Please select an Excel (.xlsx) file");
        return;
      }

      // Tạo FormData
      const formData = new FormData();
      formData.append("file", file);

      // Gửi file đến server
      const response = await axios.post(
        `${mailApiService.baseUrl}/mail-excel/import`,
        formData,
        {
          headers: {
            Authorization: `Bearer ${mailApiService.getToken()}`,
            "Content-Type": "multipart/form-data",
          },
        }
      );

      if (response.data.success) {
        alert(`Import completed:\n${response.data.message}`);
        // Xóa cache và làm mới dữ liệu
        // sessionStorage.removeItem("filterOptions");
        await filterHandler.loadFilterOptions();
        await loadData();
      } else {
        alert("Import failed: " + response.data.message);
      }
    } catch (error) {
      console.error("Import error:", error);
      alert(
        "Error importing file: " +
          (error.response?.data?.message || error.message)
      );
    }
  });

  // Mở hộp thoại chọn file
  $fileInput.click(); // Đặt ở cuối để đảm bảo mọi thứ đã sẵn sàng
}
