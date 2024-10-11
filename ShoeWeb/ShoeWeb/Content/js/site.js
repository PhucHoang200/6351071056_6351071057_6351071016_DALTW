document.addEventListener("DOMContentLoaded", function () {
    const carousel = document.querySelector('#carouselExampleCaptions');
    const items = carousel.querySelectorAll('.carousel-item');
    const buttonIndex = carousel.querySelectorAll('');
    let currentIndex = 0;
    const totalItems = items.length;

    function showSlide(index) {
        items[currentIndex].classList.remove('active');
        items[index].classList.add('active');
        currentIndex = index;
    }

    function nextSlide() {
        let nextIndex = currentIndex + 1;
        if (nextIndex >= totalItems) {
            nextIndex = 0;
        }
        showSlide(nextIndex);
    }

    setInterval(nextSlide, 3000); 
});
