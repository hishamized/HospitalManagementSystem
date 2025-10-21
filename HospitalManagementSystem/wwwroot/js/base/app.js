$(document).ready(function () {

    // --------- Enable Notification Sound
    var EnableNotificationSoundSwitch;
    var EnableNotificationSoundItem = $('#EnableNotificationSound')[0];
    var NotificationSound = localStorage.getItem('EnableNotificationSound');

    if (NotificationSound === 'false') {
        EnableNotificationSoundItem.checked = false;
    } else {
        EnableNotificationSoundItem.checked = true;
    }

    EnableNotificationSoundSwitch = new Switchery(EnableNotificationSoundItem, { size: 'small' });

    $(EnableNotificationSoundItem).change(function () {
        localStorage.setItem('EnableNotificationSound', this.checked);
    });



    // --------- Enable Notification
    var EnableNotificationSwitch;
    var EnableNotificationItem = $('#EnableNotification')[0];

    if (localStorage.getItem('EnableNotification') === 'false') {
        EnableNotificationItem.checked = false;
        EnableNotificationSoundSwitch.disable();
    } else {
        // if null or true
        EnableNotificationItem.checked = true;
        EnableNotificationSoundSwitch.enable();
    }

    EnableNotificationSwitch = new Switchery(EnableNotificationItem, { size: 'small' });

    $(EnableNotificationItem).change(function () {

        localStorage.setItem('EnableNotification', this.checked);
        if (this.checked === true) {
            EnableNotificationSoundSwitch.enable()
        } else {
            EnableNotificationSoundSwitch.disable();
        }
    });



    // dragula - drag and drop
    var drake = dragula([$('#dragula-container')[0]], {
        revertOnSpill: false,
        moves: function (el, source, handle, sibling) {
            return handle.classList.contains('move-handle');
        }
    });

    function LoadSavedConfig() {
        if (localStorage.getItem('widgetOrder') !== null) {
            widgetOrder = JSON.parse(localStorage.getItem('widgetOrder'));

            widgetOrder.forEach(function (widget) { document.getElementById('dragula-container').appendChild(document.getElementById(widget)); })
        }
    }

    LoadSavedConfig();

    drake.on('drop', function (el, target, source, sibling) {
        var widgetOrder = [];
        $('#dragula-container').children().each(function (index, el) {
            widgetOrder.push(el.id);
        });

        localStorage.setItem('widgetOrder', JSON.stringify(widgetOrder));
    });

    // card-header switchery
    var items = {};
    $('.statusItems').each(function (i, el) {
        items[el.id] = $('#' + el.id)[0];
    });

    var widgetsStatus = JSON.parse(localStorage.getItem('widgetsStatus'));

    if (widgetsStatus !== null) {
        widgetsStatus.forEach(function (obj, i) {

            items[obj.CheckBoxID].checked = obj.CheckBoxValue;
            if (obj.CheckBoxValue === false) {
                var CardBody = '#' + obj.CheckBoxID + 'CardBody';
                $(CardBody).hide();
            }
        });
    }

    var switchery = {};
    $.each(items, function (id, item) {
        switchery[id] = new Switchery(item, { size: 'small' });
    });

    $('.statusItems').change(function () {
        var statusItems = [];
        $('.statusItems').each(function (index, el) {
            statusItems.push({ "CheckBoxID": el.id, "CheckBoxValue": el.checked });
        });

        localStorage.setItem('widgetsStatus', JSON.stringify(statusItems));

        var CardBody = '#' + this.id + 'CardBody';
        $(CardBody).toggle();

    });
});
