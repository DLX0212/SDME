
// JavaScript personalizado para D' Méndez Empanadas

// ========================================
// ACTUALIZAR BADGE DEL CARRITO
// ========================================
function actualizarBadgeCarrito() {
    fetch('/Carrito/CantidadItems')
        .then(response => response.json())
        .then(data => {
            const badge = document.getElementById('carrito-badge');
            if (badge) {
                badge.textContent = data.cantidad || 0;

                // Animación cuando cambia la cantidad
                if (data.cantidad > 0) {
                    badge.classList.add('animate__animated', 'animate__bounce');
                    setTimeout(() => {
                        badge.classList.remove('animate__animated', 'animate__bounce');
                    }, 1000);
                }
            }
        })
        .catch(error => console.error('Error al actualizar carrito:', error));
}

// Actualizar badge al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    actualizarBadgeCarrito();
});

// ========================================
// CONFIRMACIÓN DE ELIMINACIÓN
// ========================================
function confirmarEliminacion(mensaje) {
    return confirm(mensaje || '¿Estás seguro de eliminar este elemento?');
}

// ========================================
// FORMATEO DE NÚMEROS
// ========================================
function formatearPrecio(precio) {
    return new Intl.NumberFormat('es-DO', {
        style: 'currency',
        currency: 'DOP'
    }).format(precio);
}

// ========================================
// FORMATEO DE FECHA
// ========================================
function formatearFecha(fecha) {
    return new Date(fecha).toLocaleDateString('es-DO', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
}

// ========================================
// VALIDACIÓN DE TELÉFONO DOMINICANO
// ========================================
function validarTelefono(telefono) {
    // Formato: (809/829/849) XXX-XXXX
    const regex = /^(\(?(809|829|849)\)?[\s.-]?)?\d{3}[\s.-]?\d{4}$/;
    return regex.test(telefono);
}

// ========================================
// VALIDACIÓN DE CÉDULA DOMINICANA
// ========================================
function validarCedula(cedula) {
    // Formato: XXX-XXXXXXX-X
    const regex = /^\d{3}-?\d{7}-?\d{1}$/;
    return regex.test(cedula);
}

// ========================================
// AUTO-DISMISS ALERTS
// ========================================
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');

    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000); // 5 segundos
    });
});

// ========================================
// SMOOTH SCROLL
// ========================================
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// ========================================
// LAZY LOADING DE IMÁGENES
// ========================================
document.addEventListener('DOMContentLoaded', function () {
    const images = document.querySelectorAll('img[data-src]');

    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.removeAttribute('data-src');
                observer.unobserve(img);
            }
        });
    });

    images.forEach(img => imageObserver.observe(img));
});

// ========================================
// TOOLTIPS DE BOOTSTRAP
// ========================================
document.addEventListener('DOMContentLoaded', function () {
    const tooltipTriggerList = [].slice.call(
        document.querySelectorAll('[data-bs-toggle="tooltip"]')
    );
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// ========================================
// POPOVERS DE BOOTSTRAP
// ========================================
document.addEventListener('DOMContentLoaded', function () {
    const popoverTriggerList = [].slice.call(
        document.querySelectorAll('[data-bs-toggle="popover"]')
    );
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});

// ========================================
// BÚSQUEDA EN TIEMPO REAL
// ========================================
function setupLiveSearch(inputId, targetId) {
    const searchInput = document.getElementById(inputId);
    const targetElements = document.querySelectorAll(targetId);

    if (searchInput && targetElements.length > 0) {
        searchInput.addEventListener('input', function () {
            const searchTerm = this.value.toLowerCase();

            targetElements.forEach(element => {
                const text = element.textContent.toLowerCase();
                if (text.includes(searchTerm)) {
                    element.style.display = '';
                } else {
                    element.style.display = 'none';
                }
            });
        });
    }
}

// ========================================
// LOADING OVERLAY
// ========================================
function mostrarLoading() {
    const overlay = document.createElement('div');
    overlay.id = 'loadingOverlay';
    overlay.className = 'position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center';
    overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
    overlay.style.zIndex = '9999';
    overlay.innerHTML = `
        <div class="spinner-border text-danger" role="status" style="width: 3rem; height: 3rem;">
            <span class="visually-hidden">Cargando...</span>
        </div>
    `;
    document.body.appendChild(overlay);
}

function ocultarLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.remove();
    }
}

// ========================================
// COPIAR AL PORTAPAPELES
// ========================================
function copiarAlPortapapeles(texto) {
    navigator.clipboard.writeText(texto).then(function () {
        alert('Copiado al portapapeles');
    }, function (err) {
        console.error('Error al copiar:', err);
    });
}

// ========================================
// VALIDACIÓN DE FORMULARIOS
// ========================================
(function () {
    'use strict';

    // Validación de formularios con Bootstrap
    const forms = document.querySelectorAll('.needs-validation');

    Array.from(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        }, false);
    });
})();

// ========================================
// PREVENIR DOBLE SUBMIT
// ========================================
document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function () {
            const submitButtons = form.querySelectorAll('button[type="submit"]');
            submitButtons.forEach(button => {
                button.disabled = true;
                button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Procesando...';
            });
        });
    });
});

// ========================================
// NOTIFICACIONES TOAST
// ========================================
function mostrarToast(mensaje, tipo = 'info') {
    // tipo: 'success', 'error', 'warning', 'info'
    const toastContainer = document.getElementById('toastContainer');

    if (!toastContainer) {
        const container = document.createElement('div');
        container.id = 'toastContainer';
        container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(container);
    }

    const toastId = 'toast_' + Date.now();
    const bgClass = {
        'success': 'bg-success',
        'error': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    }[tipo] || 'bg-info';

    const toastHTML = `
        <div id="${toastId}" class="toast ${bgClass} text-white" role="alert">
            <div class="toast-header ${bgClass} text-white">
                <strong class="me-auto">D' Méndez</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${mensaje}
            </div>
        </div>
    `;

    document.getElementById('toastContainer').insertAdjacentHTML('beforeend', toastHTML);

    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement);
    toast.show();

    toastElement.addEventListener('hidden.bs.toast', function () {
        toastElement.remove();
    });
}

// ========================================
// EXPORTAR FUNCIONES GLOBALES
// ========================================
window.SDME = {
    actualizarBadgeCarrito,
    confirmarEliminacion,
    formatearPrecio,
    formatearFecha,
    validarTelefono,
    validarCedula,
    mostrarLoading,
    ocultarLoading,
    copiarAlPortapapeles,
    mostrarToast
};
