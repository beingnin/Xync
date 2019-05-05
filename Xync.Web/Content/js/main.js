//Sample Xync Timer Function
(function()  {
    let checkTime = i => { return (i < 10) ? "0" + i : i };
    let startTime = function()  {
        var today = new Date(),
            h = checkTime(today.getHours()),
            m = checkTime(today.getMinutes()),
            s = checkTime(today.getSeconds());
        document.querySelector('.xync-time').innerHTML = h + ":" + m + ":" + s;
        t = setTimeout(() => startTime(), 500);
    }
    startTime();
})();

var app = {

    xync: {
        errors: {
            refresh: function () {
                $.ajax({
                    url: '/Home/GetErrors',
                    method: 'GET',
                    dataType: 'html',
                    success: (data) => {
                        $('#errorsContainer').html(data);
                    },
                    error: function (err, xhr) {
                        app.message.error('Error', 'Sorrys');
                    }
                });
            }
        }
    },
    message: {
        success:function (msg, title = '')  {
            alert(msg);
        },
        error: function(msg, title = '')  {
            alert(msg);
        }
    },
    util: {
        copy: function(str) {
            const el = document.createElement('textarea');
            el.value = str;
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);
        }
    }
}