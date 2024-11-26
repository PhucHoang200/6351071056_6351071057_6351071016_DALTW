$(document).ready(function () {
    const provinceApi = 'https://vn-public-apis.fpo.vn/provinces/getAll?limit=-1';
    const districtApi = 'https://vn-public-apis.fpo.vn/districts/getByProvince';
    const wardApi = 'https://vn-public-apis.fpo.vn/wards/getByDistrict';

    // Load danh sách Tỉnh/Thành phố
    $.ajax({
        url: provinceApi,
        method: 'GET',
        success: function (response) {
            const provinceDropdown = $('#province');
            provinceDropdown.html('<option value="">Chọn Tỉnh/Thành phố...</option>');

            if (response.data && response.data.data) {
                $.each(response.data.data, function (index, province) {
                    provinceDropdown.append(
                        $('<option>', {
                            value: province.code, // Mã tỉnh
                            text: province.name  // Tên tỉnh
                        })
                    );
                });
            } else {
                console.error('Province data is invalid.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi lấy danh sách Tỉnh/Thành phố:', error);
        }
    });

    // Lắng nghe sự kiện khi chọn Tỉnh/Thành phố
    $('#province').change(function () {
        const provinceCode = $(this).val();
        const districtDropdown = $('#district');
        const wardDropdown = $('#ward');

        // Reset Quận/Huyện và Phường/Xã
        districtDropdown.html('<option value="">Chọn Quận/Huyện...</option>').prop('disabled', true);
        wardDropdown.html('<option value="">Chọn Phường/Xã...</option>').prop('disabled', true);

        // Nếu không chọn Tỉnh/Thành phố
        if (!provinceCode) return;

        // Gọi API để lấy danh sách Quận/Huyện
        $.ajax({
            url: `${districtApi}?provinceCode=${provinceCode}&limit=-1`,
            method: 'GET',
            success: function (response) {
                districtDropdown.prop('disabled', false);

                if (response.data && response.data.data) {
                    $.each(response.data.data, function (index, district) {
                        districtDropdown.append(
                            $('<option>', {
                                value: district.code, // Mã quận/huyện
                                text: district.name  // Tên quận/huyện
                            })
                        );
                    });
                } else {
                    console.error('District data is invalid.');
                }
            },
            error: function (xhr, status, error) {
                console.error('Lỗi khi lấy danh sách Quận/Huyện:', error);
            }
        });
    });

    // Lắng nghe sự kiện khi chọn Quận/Huyện
    $('#district').change(function () {
        const districtCode = $(this).val();
        const wardDropdown = $('#ward');

        // Reset Phường/Xã
        wardDropdown.html('<option value="">Chọn Phường/Xã...</option>').prop('disabled', true);

        // Nếu không chọn Quận/Huyện
        if (!districtCode) return;

        // Gọi API để lấy danh sách Phường/Xã
        $.ajax({
            url: `${wardApi}?districtCode=${districtCode}&limit=-1`,
            method: 'GET',
            success: function (response) {
                wardDropdown.prop('disabled', false);

                if (response.data && response.data.data) {
                    $.each(response.data.data, function (index, ward) {
                        wardDropdown.append(
                            $('<option>', {
                                value: ward.code, // Mã phường/xã
                                text: ward.name  // Tên phường/xã
                            })
                        );
                    });
                } else {
                    console.error('Ward data is invalid.');
                }
            },
            error: function (xhr, status, error) {
                console.error('Lỗi khi lấy danh sách Phường/Xã:', error);
            }
        });
    });
});
