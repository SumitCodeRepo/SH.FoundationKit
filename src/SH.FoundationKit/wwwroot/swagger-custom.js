(function () {
    window.addEventListener("load", function () {
        const ui = window.ui;

        if (!ui) return;

        const headers = {};

        // Add custom "Set Headers" button
        const btn = document.createElement("button");
        btn.innerText = "Set Headers";
        btn.classList.add("btn", "authorize", "custom-set-headers");

        btn.onclick = function () {
            const key = prompt("Header name (e.g., X-Tenant-ID):");
            const value = prompt(`Value for ${key}:`);
            if (key && value) {
                headers[key] = value;
                alert(`Header set: ${key}: ${value}`);
            }
        };

        const authorizeBtn = document.querySelector(".topbar .auth-wrapper .authorize");
        if (authorizeBtn?.parentNode) {
            authorizeBtn.parentNode.appendChild(btn);
        }

        // Hook into all requests
        ui.getConfigs().requestInterceptor = (req) => {
            Object.entries(headers).forEach(([key, val]) => {
                req.headers[key] = val;
            });
            return req;
        };
    });
})();