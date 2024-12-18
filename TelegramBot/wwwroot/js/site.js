
//function filterUsers() {
//    // Arama kutusundaki değeri al
//    var input = document.getElementById("searchInput");
//    var filter = input.value.toLowerCase();

//    // Kullanıcı listesindeki tüm div'leri al
//    var userList = document.getElementById("memberList");
//    var userDivs = userList.getElementsByClassName("custom-control");

//     // Her div'de arama yap
//    for (var i = 0; i < userDivs.length ; i++) {
//        var label = userDivs[i].getElementsByTagName("label")[0];
//        var txtValue = label.textContent || label.innerText;

//        // Eşleşme varsa div'i göster, yoksa gizle
//        if (txtValue.toLowerCase().indexOf(filter) > -1) {
//            userDivs[i].style.display = "";
//        }
//        else {
//             userDivs[i].style.display = "none";
//        }
//    }
//}



//ChatMember ile Member karşılaştırması fonkisyonu
$(document).on('click', '.confirmMember', function () {
    var firstName = $(this).data('name'); // Butondan alınan ad
    var lastName = $(this).data('surname'); // Butondan alınan soyad
    var chatMemberId = $(this).data('chatmemberid');

    console.log("kabul tuşuna basıldı")

    // Ajax ile veriyi sunucuya gönder
    $.ajax({
        url: '/Admin/GetMatchingMember', // Doğru controller ve action yönlendirmesi
        method: 'POST',
        data: { firstName: firstName, lastName: lastName },
        success: function (response) {
            // Modal içindeki üye listesini güncelle
            var membersList = $('#membersList');
            membersList.empty(); // Önceki verileri temizle

            // Gelen verileri listele
            if (response.length > 0) {
                response.forEach(function (member) {
                    membersList.append(
                        '<input type="hidden" name="ChatMemberId" value="' + chatMemberId + ' "/> ' +
                        '<div class="custom-control custom-checkbox">' +
                        '<label for="user-' + member.id + '">' + member.name + ' ' + member.surname + '<br/>' + member.email + '</label>' +
                        '<input type="radio" class="custom-control-input" name="Id" value="' + member.id + '" id="user-' + member.id + '" style="float:right" />' +
                        '</div>'
                    );
                });
            } else {
                membersList.append('<p>Şirkette bu isimde biri bulunamadı</p>');
            }

            // Modal'ı aç
            $('#confirmMemberModal').modal('show');

           
        },
        error: function (error) {
            console.log('Hata:', error);
        }
    });
});

$(".deleteChatMemberBtn").on("click", function () {

    var chatmemberID = $(this).data('id');
    var username = $(this).data('name');
    var usersurname = $(this).data('surname');

    var confirmDelete = confirm('Bu kullanıcıyı silmek istediğinize emin misiniz' + username + usersurname + '?');

    if (confirmDelete) {
        $.ajax({
            url: '/Admin/DeleteChatMember',
            type: 'POST',
            data: { id: chatmemberID },
            success: function (response) {
                alert("Kullanıcı başarıyla silindi.");
                location.reload(); // Sayfayı yenile
            },
            error: function (error) {
                alert("Bir hata oluştu.");
            }
        });
    }
});

$('#confirmMemberForm').on('submit', function (e) {
    e.preventDefault(); // Formun standart gönderimini engelliyoruz

    var chatMemberId = $('input[name="ChatMemberId"]').val();
    var selectedMemberId = $('input[name="Id"]:checked').val(); // Seçilen radio butondan ID'yi al

    if (selectedMemberId) {
        // Ajax ile ConfirmMember isteği gönderiliyor
        $.ajax({
            url: '/Admin/ConfirmMember',
            method: 'POST',
            data: { chatMemberId: chatMemberId, Id: selectedMemberId },
            success: function (response) {
                $('#addTaskModal').modal('hide');
                location.reload();
                alert('Üye başarıyla seçildi.');
            },
            error: function (error) {
                console.log('Hata:', error);
            }
        });
    } else {
        alert('Lütfen bir üye seçin.');
    }
});


/*GetMember START */

//kullanıcı edit JS'leri
$(document).ready(function () {
    //$('.edit-button').on('click', function () {
    //    var id = $(this).data('id');
    //    var type = $(this).data('type');

    //    // AJAX ile veri yükleme
    //    $.ajax({
    //        url: '/Admin/GetEditForm', // Controller ve Action yönlendirmesi
    //        method: 'GET',
    //        data: { id: id, type: type },
    //        success: function (response) {
    //            // Modal içeriğini güncelle
    //            $('#modalContent').html(response);
    //            // Modal'ı aç
    //            $('#editModal').modal('show');
    //        },
    //        error: function (error) {
    //            console.log('Hata:', error);
    //        }
    //    });
    //});

    $('#createMemberBtn').on('click', function () {
        $.ajax({
            url: '/Admin/GetCategory',
            method: 'GET',        

            success: function (dataCategory) {
                // Veriler başarılı şekilde geldiyse modalı doldur
                var selectedCategory = $("#category");
                selectedCategory.empty();

                selectedCategory.append(
                    '<option selected>Çalışan Kategorisini Seçiniz</option>',
                );

                dataCategory.forEach(function (category) {
                    selectedCategory.append(
                        '<option value="' + category.name + '">' + category.name + '</option>'
                    );
                });

                $('#memberModal').modal('show');
            },


            error: function (err) {

                alert("Kategorilerin listelenmesi sırasında bir hata oluştu.");
            }
        })
    })

    $('.delete-button').on('click', function () {
        var id = $(this).data('id');
        var type = $(this).data('type');
        var confirmation = confirm('Bu kaydı silmek istediğinizden emin misiniz?');

        if (confirmation) {
            // Silme işlemi için AJAX çağrısı yap
            $.ajax({
                url: (type === 'User' ? '/Admin/DeleteUser' : '/Admin/DeleteMember'), // Controller ve Action yönlendirmesi
                method: 'POST',
                data: { id: id },
                success: function (response) {
                    // Başarılı olduğunda pop-up göster
                    alert('Kayıt başarıyla silindi.');
                    location.reload(); // Sayfayı yenile
                },
                error: function (error) {
                    console.log('Hata:', error);
                    alert('Silme işlemi sırasında bir hata oluştu.');
                }
            });
        }
    });

});

//modal'dan açılan formdaki verilerle Create
$(document).ready(function () {
    $("#createMemberForm").on("submit", function (e) {
        e.preventDefault();

        var formData = {
            Name: $("#Name").val(),
            Surname: $("#Surname").val(),
            startingJob: $("#startingJob").val(),
            telegramId: $("#telegramId").val(),
            phone: $("#phone").val(),
            email: $("#email").val(),
            category: $("#category").val(),
        };

        $.ajax({
            url: '/Admin/CreateMember',
            type: 'POST',
            data: formData,
            success: function (response) {
                // Başarılı olursa modal kapansın ve listeyi yenile
                $('#memberModal').modal('hide');
                location.reload();
            },
            error: function (response) {
                // Hata durumunda mesaj ver
                alert("Bir hata oluştu!");
            }
        });
    });
});

$(document).ready(function () {

    // Düzenle butonuna tıklandığında bilgileri modal içine doldur
    $(".open-model").on("click", function () {
        var memberId = $(this).attr("route-id");

        // Üyenin bilgilerini almak için AJAX kullanarak backend'den veriyi çekeceğiz
        $.ajax({
            url: '/Admin/GetMemberById', // Üye bilgilerini çekeceğimiz endpoint
            type: 'GET',
            data: { id: memberId },
            success: function (data) {
                // Veriler başarılı şekilde geldiyse modalı doldur
                $("#editMemberId").val(data.id);
                $("#editName").val(data.name);
                $("#editSurname").val(data.surname);
                $("#editStartingJob").val(data.startingJob.substring(0, 10)); // Date'in ilk 10 karakteri lazım
                $("#editTelegramId").val(data.telegramId);
                $("#editPhone").val(data.phone);
                $("#editEmail").val(data.email);

                var memberCategory = data.category;


                $.ajax({
                    url: '/Admin/GetCategory', // category çek
                    type: 'GET',
                    
                    success: function (dataCategory) {
                        // Veriler başarılı şekilde geldiyse modalı doldur
                        var selectedCategory = $("#editCategory");
                        selectedCategory.empty();

                        dataCategory.forEach(function (category) {
                            selectedCategory.append(
                                '<option value="' + category.name + '">' + category.name + '</option>'
                            );
                        });

                        selectedCategory.val(memberCategory)
                                          
                    },
                    error: function (err) {

                        alert("Kategorilerin listelenmesi sırasında bir hata oluştu.");
                    }
                });

                // Modal'ı aç
                $('#editMemberModal').modal('show');


            },
            error: function (err) {
                
                alert("Veri çekme sırasında bir hata oluştu.");
            }
        });


    });

    // Formu submit ettiğimizde backend'e gönder
    $("#editMemberForm").on("submit", function (e) {
        e.preventDefault();

        var formData = {
            Id: $("#editMemberId").val(),
            Name: $("#editName").val(),
            Surname: $("#editSurname").val(),
            startingJob: $("#editStartingJob").val(),
            telegramId: $("#editTelegramId").val(),
            phone: $("#editPhone").val(),
            email: $("#editEmail").val(),
            category: $("#editCategory").val(),
        };

        $.ajax({
            url: '/Admin/updateMember',
            type: 'POST',
            data: formData,
            success: function (response) {
                // Başarılı olursa modal kapansın ve listeyi yenile
                $('#editMemberModal').modal('hide');
                location.reload();
            },
            error: function (response) {
                // Hata durumunda mesaj ver
                alert("Güncelleme sırasında bir hata oluştu!");
            }
        });
    });

    $(".connectUser").on("click", function () {
        var memberEmail = $(this).data('email');
        var memberId = $(this).data('memberid');

        $.ajax({
            url: "/Admin/GetMatchingUser",
            method: 'GET',
            data: { email: memberEmail },

            success: function (response) {
                var userList = $('#userList');
                userList.empty(); // Önceki verileri temizle

                if (response.length > 0) {
                    response.forEach(function (user) {
                        userList.append(
                            '<input type="hidden" name="memberId" value="' + memberId + ' "/> ' +
                            '<div class="custom-control custom-checkbox">' +
                            '<label for="user-' + user.id + '">' + user.userName + ' ' + user.userSurname + '<br/>' + user.userEmail + '</label>' +
                            '<input type="radio" class="custom-control-input" name="Id" value="' + user.id + '" id="user-' + user.id + '" style="float:right" />' +
                            '</div>'
                        );
                    });
                } else {
                    userList.append('<p>Şirkette bu çalışana ait bir hesap bulunamadı</p>');
                }

                // Modal'ı aç
                $('#confirmUserModal').modal('show');
            },

            error: function (error) {
                alert("Bu çalışana ait bir hesap bulunamadı.")
                
            }

        });

    });

    $('#confirmUserForm').on('submit', function (e) {
        e.preventDefault(); // Formun standart gönderimini engelliyoruz

        var memberId = $('input[name="memberId"]').val();
        var selectedUserId = $('input[name="Id"]:checked').val(); // Seçilen radio butondan ID'yi al

        if (selectedUserId) {
            // Ajax ile ConfirmMember isteği gönderiliyor
            $.ajax({
                url: '/Admin/ConfirmUser',
                method: 'POST',
                data: { memberId: memberId, userId: selectedUserId },
                success: function (response) {
                    $('#confirmUserModal').modal('hide');
                    location.reload();
                    alert('Çalışan hesabı başarıyla ilişkilendirildi.');
                },
                error: function (xhr, status, error) {
                    // Hata mesajını al ve göster
                    if (xhr.responseText) {
                        alert(xhr.responseText);
                    } else {
                        alert("Bir hata oluştu.");
                    }
                }
            });
        } else {
            alert('Lütfen bir üye seçin.');
        }
    });




});

/*GetMember FINISH */


/* TaskTable Start */

$(document).on('click', '.openAddTaskModal', function () {
    // Fonksiyonun içine girdiğini kontrol etmek için buraya log ekleyin
    console.log('Butona tıklandı!');

    $.ajax({
        url: '/Home/GetMembers',
        method: 'GET',
        success: function (response) {

            $.ajax({
                url: '/Home/GetCategories',
                type: 'GET',

                success: function (categories) {

                    var categorySelect = $('#memberCategories');
                    categorySelect.empty();
                    categorySelect.append('<option value="">Kategori Seçiniz</option>');

                    categories.forEach(function (category) {
                        categorySelect.append(`<option value="${category.name}">${category.name}</option>`);
                    });
                },


                error: function (xhr, status, error) {
                    if (xhr.responseText) {
                        alert(xhr.responseText);
                    } else {
                        alert("Bir hata oluştu.");
                    }
                }

            });


            console.log(response); // Gelen yanıtı kontrol edin
            var memberList = $('#membersListCheckbox');
            memberList.empty();

            if (response.length > 0) {
                response.forEach(function (member) {
                    memberList.append(

                        '<input type="checkbox" class="custom-control-input mb-2" id="user-' + member.id + '" value="' + member.id + '" style="float:right" />' +
                        '<label class="custom-control-label mb-2" for="user-' + member.id + '">' + member.name + ' ' + member.surname + '</label>' +
                        '</br>'
                        
                    );
                });
            } else {
                memberList.append('<p>Üye bulunamadı.</p>');
            }

            $('#addTaskModal').modal('show');
        },
        error: function (error) {
            console.log('Hata:', error);
        }
    });
});



$(document).ready(function () {
    // Kaydet butonuna tıklama olayını dinle
    $('#saveTaskBtn').on('click', function () {
        // Modal içindeki input değerlerini al
        var projectName = $('input[name="project-name"]').val();
        var projectDescription = $('textarea[name="project-description"]').val();
        var startDate = $('input[name="project-start-date"]').val();
        var endDate = $('input[name="project-end-date"]').val();
        var projectGiver = $('input[name="project-giver"]').val();

        // Seçilen kullanıcıları al
        var selectedUserIds = [];
        $('#membersList input[type="checkbox"]:checked').each(function () {
            selectedUserIds.push(parseInt($(this).attr('id').split('-')[1])); // CheckBox ID'sinden kullanıcı ID'sini al
        });

        // Task nesnesini oluştur
        var Task = {
            taskName: projectName,
            taskDescription: projectDescription,
            startingTime: startDate,
            endingTime: endDate,
            taskGiver: projectGiver
        };
        console.log(Task);
        console.log(selectedUserIds);

        // Ajax ile verileri gönder
        $.ajax({
            url: '/Home/AssignTask', // Controller'daki endpoint URL'si
            type: 'POST',
            data: {
                Task, membersId: selectedUserIds
            },
                // Seçilen kullanıcılar dizi olarak gönderiliyor
            

            

         
            success: function (response) {
                // Başarılı istek sonrası işlem
                alert('Görev başarıyla kaydedildi.');
                // Modalı kapatma (isteğe bağlı)
                $('#addTaskModal').modal('hide');
            },
            error: function (error) {
                console.log('Hata:', error); // Hata ayrıntılarını konsola yazdır
                alert('Bir hata oluştu: ' + error.responseText);
            }
        });
    });
});

$(document).ready(function () {
    $.ajax({
        url: '/Home/TaskOnGoing',
        type: 'GET',

        success: function (response) {
            var taskOnGoing = $('#OnGoingTableBody');
            taskOnGoing.empty();

            if (response.length > 0) {
                response.forEach(function (task) {
                    taskOnGoing.append(
                        '<tr>' +
                        '<td>' + task.taskGiver + '</td>' +
                        '<td>' + task.taskName + '</td>' +
                        '<td>' + new Date(task.startingTime).toLocaleDateString() + '</td>' +
                        '</tr > '
                    );
                })
            }
        }
    });


    $.ajax({
        url: '/Home/TaskFuture',
        type: 'GET',

        success: function (response) {
            var taskOnGoing = $('#futureTableBody');
            taskOnGoing.empty();

            if (response.length > 0) {
                response.forEach(function (task) {
                    taskOnGoing.append(
                        '<tr>' +
                        '<td>' + task.taskGiver + '</td>' +
                        '<td>' + task.taskName + '</td>' +
                        '<td>' + new Date(task.startingTime).toLocaleDateString() + '</td>' +
                        '</tr > '
                    );
                })
            }
        }
    });


    $.ajax({
        url: '/Home/TaskUserList',
        type: 'GET',

        success: function (response) {
            var taskOnGoing = $('#taskUserList');
            taskOnGoing.empty();




            if (response.length > 0) {
                response.forEach(function (task) {
                    var progressClass = '';
                    var progressBadge = '';

                    if (task.progress === "Başlanmadı") {
                        progressClass = 'bg-danger';// Eğer "Başlanmadı"ysa kırmızı yapalım
                        progressBadge = 'badge-danger';
                    }
                    else if (task.progress === "Devam Ediyor") {
                        progressClass = 'bg-warning'; // Devam ediyorsa sarı
                        progressBadge = 'badge-warning';
                    }
                    else {
                        progressClass = 'bg-success'; // Tamamlanmışsa yeşil
                        progressBadge = 'badge-success';
                    }

                    taskOnGoing.append(
                        '<tr>' +
                        '<td class="py-1">' + '<img src="../../assets/images/faces/face1.jpg" alt="image"/>' + '</td>' +
                        '<td>' + task.memberName + task.memberSurname + '</td>' +
                        '<td>' + task.taskName + '</td>' +
                        '<td>' + '<div class="progress">' + '<div class="progress-bar ' + progressClass + '" role="progressbar" style="width:' + task.progressPercent + '%" aria-valuenow="' + task.progressPercent + '" aria-valuemin="0" aria-valuemax="100"> ' + '</div>' + '</div>' + '</td>' +
                        '<td>' + '<label class="badge ' + progressBadge + '">' + task.progress + '</label>' + '</td>' +

                        '<td>' + new Date(task.taskDeadline).toLocaleDateString() + '</td>' +
                        '</tr > '
                    );
                })
            }
        }
    });

   

});

/* TaskTable Finish */




/* User START*/

$(document).ready(function () {
    $('.createUserBtn').on('click', function () {
        $.ajax({
            url: '/Admin/GetRole',
            method: 'GET',

            success: function (dataRole) {
                // Veriler başarılı şekilde geldiyse modalı doldur
                var selectedRole = $("#role");
                selectedRole.empty();

                selectedRole.append(
                    '<option selected>Kullanıcı Rolünü Seçiniz</option>',
                );

                dataRole.forEach(function (role) {
                    selectedRole.append(
                        '<option value="' + role.role + '">' + role.role + '</option>'
                    );
                });

                $('#userModal').modal('show');
            },


            error: function (err) {

                alert("Kategorilerin listelenmesi sırasında bir hata oluştu.");
            }
        })
    });


    $("#createUserForm").on("submit", function (e) {
        e.preventDefault();

        var Password = $("#password").val();
        var Repassword = $("#repassword").val();

        if (Password == Repassword) {
            var formData = {
                firstName: $("#Name").val(),
                lastName: $("#Surname").val(),
                phone: $("#phone").val(),
                email: $("#email").val(),
                role: $("#role").val(),
                password: Password,
                repassword: Repassword
            };

            $.ajax({
                url: '/Admin/CreateUser',
                type: 'POST',
                data: formData,
                success: function (response) {
                    // Başarılı olursa modal kapansın ve listeyi yenile
                    $('#userModal').modal('hide');
                    location.reload();
                },
                error: function (response) {
                    // Hata durumunda mesaj ver
                    alert("Bir hata oluştu!");
                }
            });
        }

        else {
            alert("Aynı şifreyi girdiğinizden emin olun.")
        }

       
    });




    $(".editUserBtn").on("click", function () {
        var userId = $(this).data("userid");

        // Üyenin bilgilerini almak için AJAX kullanarak backend'den veriyi çekeceğiz
        $.ajax({
            url: '/Admin/GetUserById', // Üye bilgilerini çekeceğimiz endpoint
            type: 'GET',
            data: { id: userId },
            success: function (data) {
                // Veriler başarılı şekilde geldiyse modalı doldur
                $("#editUserId").val(data.id);
                $("#editName").val(data.userName);
                $("#editSurname").val(data.userSurname);
                $("#editPhone").val(data.phone);
               
                $("#editEmail").val(data.userEmail);
                

                var userRole = data.role;


                $.ajax({
                    url: '/Admin/GetRole', // category çek
                    type: 'GET',

                    success: function (dataRole) {
                        // Veriler başarılı şekilde geldiyse modalı doldur
                        var selectedRole = $("#editRole");
                        selectedRole.empty();

                        dataRole.forEach(function (role) {
                            selectedRole.append(
                                '<option value="' + role.role + '">' + role.role + '</option>'
                            );
                        });

                        selectedRole.val(userRole)

                    },
                    error: function (err) {

                        alert("Rollerin listelenmesi sırasında bir hata oluştu.");
                    }
                });

                // Modal'ı aç
                $('#editUserModal').modal('show');


            },
            error: function (err) {

                alert("Veri çekme sırasında bir hata oluştu.");
            }
        });


    });

    // Formu submit ettiğimizde backend'e gönder
    $("#editUserModal").on("submit", function (e) {
        e.preventDefault();

        var formData = {
            Id: $("#editUserId").val(),
            Name: $("#editName").val(),
            Surname: $("#editSurname").val(),
            phone: $("#editPhone").val(),
            role: $("#editRole").val(),
            Email: $("#editEmail").val(),
            
        };

        $.ajax({
            url: '/Admin/updateUser',
            type: 'POST',
            data: formData,
            success: function (response) {
                // Başarılı olursa modal kapansın ve listeyi yenile
                $('#editUserModal').modal('hide');
                location.reload();

                alert("Güncelleme başarıyla yapıldı")
            },
            error: function (xhr, status, error) {
                // Hata mesajını al ve göster
                if (xhr.responseText) {
                    alert(xhr.responseText);
                } else {
                    alert("Bir hata oluştu.");
                }
            }
        });
    });


    $(".deleteUserBtn").on("click", function () {

        var userId = $(this).data('userid');
        var username = $(this).data('name');
        var usersurname = $(this).data('surname');

        var confirmDelete = confirm('Bu kullanıcıyı silmek istediğinize emin misiniz' + username + usersurname + '?');

        if (confirmDelete) {
            $.ajax({
                url: '/Admin/deleteUser',
                type: 'POST',
                data: { id: userId },
                success: function (response) {
                    alert("Kullanıcı başarıyla silindi.");
                    location.reload(); // Sayfayı yenile
                },
                error: function (error) {
                    alert("Bir hata oluştu.");
                }
            });
        }
    });

});

/* User FINISH*/


/* MYTASKS START */


$(document).ready(function () {

    
        $.ajax({
            url: '/Home/GetMyTasks',
            type: 'GET',
            success: function (tasks) {
                var tasksContainer = $('#tasksContainer');
                tasksContainer.empty();

                $.ajax({
                    url: '/Home/GetTaskStatus',
                    type: 'GET',
                    success: function (tasksStatus) {

                        tasks.forEach(function (task) {
                            var statusBadge = '';
                            if (task.progress === "Devam Ediyor") {
                                statusBadge = '<label class="badge badge-warning" style="float:right">Devam Ediyor</label>';
                            }

                            else if (task.progress === "Tamamlandı") {
                                statusBadge = '<label class="badge badge-success" style="float:right">Tamamlandı</label>';
                            }

                            else if (task.progress === "Beklemede") {
                                statusBadge = '<label class="badge badge-info" style="float:right">Beklemede</label>';


                            }

                            else {
                                statusBadge = '<label class="badge badge-danger" style="float:right">Başlanmadı</label>';
                            }

                            var taskStatusOptions = '';
                            tasksStatus.forEach(function (status) {
                                var selected = task.progress === status ? 'selected' : '';
                                taskStatusOptions += `<option value="${status}" ${selected}>${status}</option>`;
                            });


                            var taskCard = `
                        <div class="card col-sm-6 col-md-3 ms-5 mb-5">
                            <div class="card-body">
                                <div>
                                    <h5 class="card-title" style="display:inline-block">Proje Bilgileri</h5>
                                    ${statusBadge}
                                </div>
                                <form class="myTasksForm">

                                    <input type="hidden" name="taskId" value="${task.taskId}"/>    

                                    <div class="form-group">
                                        <label for="taskName">Proje Adı</label>
                                        <input type="text" class="form-control myTasksInput taskName" value="${task.taskName}" readonly>
                                    </div>

                                    <div class="form-group">
                                        <label for="taskGiver">Projeyi Veren</label>
                                        <input type="text" class="form-control myTasksInput taskGiver" value="${task.taskGiver}" readonly>
                                    </div>

                                    <div class="form-group">
                                        <label for="taskDescription">Proje Detayı</label>
                                        <textarea class="form-control myTasksInput taskDescription" rows="3" readonly>${task.taskDescription}</textarea>
                                    </div>

                                    <div class="form-row">
                                        <div class="form-group col-md-6">
                                            <label for="startDate">Başlangıç Tarihi</label>
                                            <input type="date" class="form-control myTasksInput startDate" value="${task.startingTime.substring(0, 10)}" readonly>
                                        </div>
                                        <div class="form-group col-md-6">
                                            <label for="endDate">Bitiş Tarihi</label>
                                            <input type="date" class="form-control myTasksInput endDate" value="${task.taskDeadline.substring(0, 10)}" readonly>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="taskStatus">Proje Durumu</label>
                                        <select class="form-control myTasksInput taskStatus" >
                                            ${taskStatusOptions}
                                        </select>
                                    </div>

                                      <div class="form-group">
                                            <label for="progressSelect">Proje Durumu:</label>
                                            <select class="form-control progressSelectVal"  id="progressSelect  ">
                                                  <option value=0 ${task.progressPercent === 0 ? 'selected' : ''}>0%</option>
                                                  <option value=10 ${task.progressPercent === 10 ? 'selected' : ''}>10%</option>
                                                  <option value=20 ${task.progressPercent === 20 ? 'selected' : ''}>20%</option>
                                                  <option value=30 ${task.progressPercent === 30 ? 'selected' : ''}>30%</option>
                                                  <option value=40 ${task.progressPercent === 40 ? 'selected' : ''}>40%</option>
                                                  <option value=50 ${task.progressPercent === 50 ? 'selected' : ''}>50%</option>
                                                  <option value=60 ${task.progressPercent === 60 ? 'selected' : ''}>60%</option>
                                                  <option value=70 ${task.progressPercent === 70 ? 'selected' : ''}>70%</option>
                                                  <option value=80 ${task.progressPercent === 80 ? 'selected' : ''}>80%</option>
                                                  <option value=90 ${task.progressPercent === 90 ? 'selected' : ''}>90%</option>
                                                  <option value=100 ${task.progressPercent === 100 ? 'selected' : ''}>100%</option>
                                            </select>
                                        </div>

                                     <button type="submit" class="btn btn-sm btn-secondary taskUpdateBtn">Proje İlerleyişini Güncelle </button>
                                </form>
                            </div>
                        </div>
                    `;

                            // Görev kartını container'a ekle
                            tasksContainer.append(taskCard);
                        });

                    },

                    error: function () {
                        alert("Görev durumları listelenirken sorun oluştu.");
                    }

                });


            },
            error: function (error) {
                console.error("Görevler yüklenirken bir hata oluştu:", error);
            }
        });
    
    // AJAX ile görev verilerini al
   


    $(document).on('submit', '.myTasksForm', function (e) {
        e.preventDefault();
        console.log("Form submit olayı tetiklendi.");

        var formData = {
            taskId: $(this).find("input[name='taskId']").val(),
            progress: $(this).find(".taskStatus").val(),
            progressPercent: $(this).find(".progressSelectVal").val(),
        };

        console.log(formData);

        $.ajax({
            url: '/Home/UpdateMyTask',
            type: 'POST',
            data: formData,
            success: function () {
                location.reload();
            },
            error: function (xhr, status, error) {
                if (xhr.responseText) {
                    alert(xhr.responseText);
                } else {
                    alert("Bir hata oluştu.");
                }
            }
        });
    });

});



/* MYTASKS FINISH */