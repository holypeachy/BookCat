function rating(value) {
    let active = 'active-star'
    let inactive = 'inactive-star'

    let s1 = document.getElementById("star-1");
    let s2 = document.getElementById("star-2");
    let s3 = document.getElementById("star-3");
    let s4 = document.getElementById("star-4");
    let s5 = document.getElementById("star-5");
    let list = [s1, s2, s3, s4, s5]

    
    list.forEach(element => {
        element.classList.remove(active);
        element.classList.add(inactive);
    });

    for (let index = 0; index < value; index++) {
        list[index].classList.remove(inactive);
        list[index].classList.add(active);
    }

    document.getElementById("hidden-input").value = value;
}