(function () {
    const KEY = "sidebar-collapsed";
    const root = document.documentElement;
    const body = document.body;
    const toggleBtn = document.getElementById("sidebarToggle");
    const toggleIcon = document.getElementById("sidebarToggleIcon");
    const openBtn = document.getElementById("sidebarOpen");
    const backdrop = document.getElementById("sidebarBackdrop");
    const reportes = document.getElementById("reportesToggle");
    const reportesMenu = document.getElementById("collapseReportes");
    const labeled = document.querySelectorAll("#sidebar-wrapper [data-label]");
    const mobile = window.matchMedia("(max-width: 767.98px)");

    const isCollapsed = () => !mobile.matches && root.classList.contains("sidebar-collapsed");

    // Sincroniza icono, aria y tooltips con el estado actual
    function sync() {
        const collapsed = isCollapsed();
        toggleIcon.classList.toggle("fa-angle-left", !collapsed);
        toggleIcon.classList.toggle("fa-angle-right", collapsed);
        toggleBtn.setAttribute("aria-expanded", String(!collapsed));
        // Con el menú colapsado solo se ven iconos: se muestra el nombre como tooltip
        labeled.forEach(el => collapsed
            ? el.setAttribute("title", el.dataset.label)
            : el.removeAttribute("title"));
    }

    function setCollapsed(value) {
        root.classList.toggle("sidebar-collapsed", value);
        try { localStorage.setItem(KEY, value ? "1" : "0"); } catch (e) { }
        sync();
    }

    // --- Escritorio: colapsar / expandir ---
    toggleBtn.addEventListener("click", function (e) {
        e.preventDefault();
        setCollapsed(!root.classList.contains("sidebar-collapsed"));
    });

    // Clic en "Reportes" con el menú colapsado: expande el menú y abre el submenú
    reportes.addEventListener("click", function (e) {
        if (!isCollapsed()) return;
        e.preventDefault();
        e.stopPropagation(); // evita que Bootstrap alterne el submenú por su cuenta
        setCollapsed(false);
        bootstrap.Collapse.getOrCreateInstance(reportesMenu, { toggle: false }).show();
    });

    // --- Móvil: menú deslizable ---
    function setMobileOpen(open) {
        body.classList.toggle("sidebar-open", open);
        backdrop.hidden = !open;
        openBtn.setAttribute("aria-expanded", String(open));
    }

    openBtn.addEventListener("click", () => setMobileOpen(true));
    backdrop.addEventListener("click", () => setMobileOpen(false));

    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape" && body.classList.contains("sidebar-open")) {
            setMobileOpen(false);
            openBtn.focus();
        }
    });

    // Al navegar desde el menú en móvil, se cierra
    document.querySelectorAll("#sidebar-wrapper a[href]:not([data-bs-toggle])").forEach(a =>
        a.addEventListener("click", () => setMobileOpen(false)));

    // Al pasar a escritorio, limpia el estado móvil
    mobile.addEventListener("change", function () {
        setMobileOpen(false);
        sync();
    });

    sync();
})();