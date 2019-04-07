//Sample Xync Timer Function
(() => {
    let checkTime = i => { return (i < 10) ? "0" + i : i };
    let startTime = () => {
        var today = new Date(),
            h = checkTime(today.getHours()),
            m = checkTime(today.getMinutes()),
            s = checkTime(today.getSeconds());
        document.querySelector('.xync-time').innerHTML =  h + ":" + m + ":" + s;
        t = setTimeout(() =>startTime(), 500);
    }
    startTime();
})();