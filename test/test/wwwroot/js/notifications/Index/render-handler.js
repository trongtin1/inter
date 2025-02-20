const renderHandler = {
  renderTable(items) {
    const tbody = $("#mailsTableBody");
    tbody.empty();

    items.forEach((notification) => {
      const row = `
        <tr>
          <td>
            <input type="checkbox" class="form-check-input notification-checkbox" value="${
              notification.id
            }" />
          </td>
          <td><a href="/Notifications/Details/${notification.id}">${
        notification.id
      }</a></td>
          <td>${formatHandler.formatDateTime(notification.createdTime)}</td>
          <td>${notification.type}</td>
          <td>${formatHandler.formatNotificationContent(
            notification.content
          )}</td>
          <td>${
            notification.isRead
              ? '<span class="badge bg-success">Read</span>'
              : '<span class="badge bg-warning">Unread</span>'
          }</td>
          <td><span class="badge bg-secondary">${
            notification.email || ""
          }</span></td>
          <td>${formatHandler.formatDateTime(notification.lastModified)}</td>
          <td style="white-space: nowrap;">${notification.from}</td>
          <td>${
            notification.url
              ? `<a href="${notification.url}" target="_blank">Link</a>`
              : ""
          }</td>
          <td>${
            notification.isSeen
              ? '<span class="badge bg-success">Seen</span>'
              : '<span class="badge bg-warning">Unseen</span>'
          }</td>
          <td>${formatHandler.formatNotificationContent(
            notification.contentEn
          )}</td>
          <td>${
            notification.urlMobile
              ? `<a href="${notification.urlMobile}" target="_blank">Mobile Link</a>`
              : ""
          }</td>
          <td>${formatHandler.formatNotificationContent(
            notification.reqBody
          )}</td>
          <td>${notification.detailId || ""}</td>
          <td>${formatHandler.formatDateTime(notification.seenTime)}</td>
          <td>${notification.hostName || ""}</td>
          <td>${notification.functionName || ""}</td>
        </tr>
      `;
      tbody.append(row);
    });

    // Reset select all checkbox state
    $("#selectAll").prop("checked", false);
  },
  renderPagination(data) {
    const pagination = $("#pagination");
    pagination.empty();
    totalPages = data.totalPages;
    currentPage = data.pageIndex;

    if (totalPages > 1) {
      // Previous button
      pagination.append(`
                <li class="page-item ${data.hasPreviousPage ? "" : "disabled"}">
                    <a class="page-link" href="#" onclick="changePage(${
                      currentPage - 1
                    })">Previous</a>
                </li>
            `);

      // Page numbers
      for (
        let i = Math.max(1, currentPage - 2);
        i <= Math.min(totalPages, currentPage + 2);
        i++
      ) {
        pagination.append(`
                    <li class="page-item ${i === currentPage ? "active" : ""}">
                        <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                    </li>
                `);
      }

      // Next button
      pagination.append(`
                <li class="page-item ${data.hasNextPage ? "" : "disabled"}">
                    <a class="page-link" href="#" onclick="changePage(${
                      currentPage + 1
                    })">Next</a>
                </li>
            `);
    }
  },
};
