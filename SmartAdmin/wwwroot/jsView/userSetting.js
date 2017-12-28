class userSetting {
    readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                //alert(e.target.result);
                $('#imageID').attr('src', e.target.result);

                
            }
            reader.readAsDataURL(input.files[0]);
        }
    }
}

var usersetting = new userSetting();