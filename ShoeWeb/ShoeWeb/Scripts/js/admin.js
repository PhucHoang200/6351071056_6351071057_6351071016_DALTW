function editItem(cateId) {
    document.getElementById(`viewRow-${cateId}`).style.display = 'none';
    document.getElementById(`editRow-${cateId}`).style.display = '';
}

function cancelEdit(cateId) {
    document.getElementById(`viewRow-${cateId}`).style.display = '';
    document.getElementById(`editRow-${cateId}`).style.display = 'none';
}

    //@*DELETE METHOD*@
    function deleteItem(id) {
        if (confirm("Bạn có chắc muốn xóa mục này không?")) {
        $.ajax({
            url: '/Category/Delete',
            type: 'POST',
            data: { id: id },
            success: function (result) {
                if (result.success) {
                    updateCategoryList(result.categories);
                } else {
                    alert("Không tìm thấy mục cần xóa!");
                }
            },
            error: function (xhr, status, error) {
                alert("Có lỗi xảy ra!");
            }
        });
        }
    }

    {/*@* ADD METHOD *@*/}
    function Add() {
        var Name = document.getElementById(`InputName`).value;
    var Description = document.getElementById(`InputDescription`).value;

    if (confirm("Bạn có chắc muốn thêm mục này không?")) {
        $.ajax({
            url: '/Category/AddCategory',
            type: 'POST',
            data: {
                description: Description,
                name: Name
            },
            success: function (result) {
                if (result.success) {
                    updateCategoryList(result.categories);
                    $('#exampleModalCenter').modal('hide');
                    $('.modal-backdrop').remove();
                } else {
                    alert("Thêm thất bại!");
                }
            },
            error: function (xhr, status, error) {
                alert("Có lỗi xảy ra!");
            }
        });
        }
    }


    //@* UPDATE METHOD *@
    function updateItem(id) {
        var updatedName = document.getElementById(`categoryName-${id}`).value;
    var updatedDescription = document.getElementById(`categoryDescription-${id}`).value;

    if (confirm("Bạn có chắc muốn cập nhật mục này không?")) {
        $.ajax({
            url: '/Category/Update',
            type: 'POST',
            data: {
                id: id,
                descriptionCate: updatedDescription,
                nameCate: updatedName
            },
            success: function (result) {
                if (result.success) {
                    updateCategoryList(result.categories);
                } else {
                    alert("Không tìm thấy mục cần cập nhật!");
                }
            },
            error: function (xhr, status, error) {
                alert("Có lỗi xảy ra!");
            }
        });
        }
    }

    //@* LOAD TABLE *@

    function updateCategoryList(categories) {
        var categoryTableBody = $('tbody');
    categoryTableBody.empty();

    $.each(categories, function (index, category) {
        categoryTableBody.append(`
             <tr id="viewRow-${category.cateId}">
                 <th scope="row">${category.cateId}</th>
                 <td>${category.cateName}</td>
                 <td>${category.cateDescription}</td>
                 <td scope="col">
                     <button type="button" class="btn btn-secondary"  onclick="editItem(${category.cateId})">
                         <i class="bi bi-pencil-square"></i> Sửa
                     </button>
                     <button type="button" class="btn btn-danger" onclick="deleteItem(${category.cateId})">
                         <i class="bi bi-trash"></i> Xóa
                     </button>
                 </td>
             </tr>
              <tr id="editRow-${category.cateId}" style="display:none;">
                 <th scope="row">${category.cateId}</th>
                 <td>
                     <input type="text" id="categoryName-${category.cateId}" value="${category.cateName}" class="form-control">
                 </td>
                 <td>
                     <input type="text" id="categoryDescription-${category.cateId}" value="${category.cateDescription}" class="form-control">
                 </td>
                 <td scope="col">
                     <button type="button" id="save" class="btn btn-secondary" onclick="updateItem(${category.cateId})">
                         <i class="bi bi-save"></i> Lưu
                     </button>
                     <button type="button" id="cancel" class="btn btn-danger" onclick="cancelEdit(${category.cateId})">
                         <i class="bi bi-x-circle"></i> Hủy
                     </button>
                 </td>
             </tr>
         `);
        });

    }