document.addEventListener('DOMContentLoaded', function () {

    const loadingScreen = document.getElementById('loadingScreen');
    if (loadingScreen) {
        setTimeout(() => {
            loadingScreen.style.opacity = '0';
            setTimeout(() => loadingScreen.remove(), 500);
        }, 800);
    }
    const navToggle = document.getElementById('navToggle');
    const navMenu = document.getElementById('navMenu');

    if (navToggle) {
        navToggle.addEventListener('click', () => {
            navMenu.classList.toggle('active');
        });
    }
    const navbar = document.getElementById('navbar');
    window.addEventListener('scroll', () => {
        if (window.scrollY > 50) {
            navbar.style.boxShadow = '0 4px 15px rgba(0,0,0,0.1)';
        }
        else {
            navbar.style.boxShadow = '0 4px 6px rgba(0,0,0,0.1)';
        }
    });
    const notifications = document.querySelectorAll('.notification');
    notifications.forEach(note => {
        setTimeout(() => {
            note.style.opacity = '0';
            note.style.transform = 'translateX(100%)';
            setTimeout(() => note.remove(), 300);
        }, 5000);
    });
});