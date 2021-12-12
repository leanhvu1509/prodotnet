//sort
const btn=document.querySelectorAll('.btn');
const wallpaper=document.querySelectorAll('.wall-item');

btn.forEach((button) => {
    button.addEventListener('click',function(e){
        e.preventDefault();//ngan link
        const filter=e.target.dataset.filter;//lay att data btn
        wallpaper.forEach((item)=>{
             if (filter === 'all') {
                 item.style.display = 'block';
             } else {
                 if (item.classList.contains(filter)) {
                     item.style.display = 'block';
                 } else {
                     item.style.display = 'none';
                 }
             }
        })
    })
});

//tim kiem
const search=document.querySelector('#search-item');

    search.addEventListener('keyup',function(e){
        const searchF=e.target.value.toLowerCase().trim();
        wallpaper.forEach((item)=>{
            console.log(item.textContent);
            if (item.textContent.toLowerCase().includes(searchF)) {
                item.style.display='block';
            } else {
                item.style.display = 'none';
            }
        });
    });

