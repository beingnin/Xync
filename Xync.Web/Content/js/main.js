//Sample Xync Timer Function
(function () {
    let checkTime = i => { return (i < 10) ? "0" + i : i };
    let startTime = function () {
        var today = new Date(),
            h = checkTime(today.getHours()),
            m = checkTime(today.getMinutes()),
            s = checkTime(today.getSeconds());
        document.querySelector('.xync-time').innerHTML = h + ":" + m + ":" + s;
        t = setTimeout(() => startTime(), 1000);
    }
    startTime();
})();

var app = {

    xync: {
        errors: {
            refresh: function (page = 0, count = 20) {
                return $.ajax({
                    url: '/Home/GetErrors?page=' + page + '&count=' + count,
                    method: 'GET',
                    dataType: 'html',
                    beforeSend: function () { $('#btnRefreshErrors i').addClass('fa-spin'); },
                    complete: function () { setTimeout(function () { $('#btnRefreshErrors i').removeClass('fa-spin');},500) },
                    success: (data) => {
                        if (page == 0) {
                            $('#errorsContainer').html('');
                        }
                        $('#errorsContainer').append(data);
                        //$('#btnLoadMoreEvents').data('page', page).data('count', count);

                    },
                    error: function (err, xhr) {
                        app.message.error('Error', 'Sorrys');
                    }
                });
            }
        },
        mappings: {
            refresh: function () {
                $.ajax({
                    url: '/Home/GetMappings',
                    method: 'GET',
                    dataType: 'html',
                    success: (data) => {
                        $('#mappingsContainer').html(data);

                    },
                    error: function (err, xhr) {
                        app.message.error('Error', 'Sorrys');
                    }
                });
            }
        }
    },
    message: {
        success: function (msg, title = '') {
            alert(msg);
        },
        error: function (msg, title = '') {
            alert(msg);
        }
    },
    util: {
        copy: function (str) {
            const el = document.createElement('textarea');
            el.value = str;
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);
        }
    }
}