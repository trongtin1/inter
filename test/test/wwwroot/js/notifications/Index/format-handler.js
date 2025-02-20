const formatHandler = {
  formatNotificationContent(content) {
    if (!content) return "";
    if (content.length > 10) {
      return `
                <div class="email-content">
                    <div class="content-preview text-truncate">
                        ${content.substring(0, 10)}...
                    </div>
                    <div class="content-full d-none">
                        ${content}
                    </div>
                </div>
            `;
    }
    return content;
  },
  formatEmailList(emails) {
    if (!emails) return "";
    return emails
      .split(";")
      .map(
        (email) =>
          `<span class="badge bg-secondary me-1">${email.trim()}</span>`
      )
      .join("");
  },
  formatDateTime(dateString) {
    if (!dateString) return "";
    const date = new Date(dateString);
    const day = date.getDate().toString().padStart(2, "0");
    const month = (date.getMonth() + 1).toString().padStart(2, "0");
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, "0");
    const minutes = date.getMinutes().toString().padStart(2, "0");
    const seconds = date.getSeconds().toString().padStart(2, "0");
    return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;
  },
};
