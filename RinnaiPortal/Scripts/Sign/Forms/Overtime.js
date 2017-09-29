$(function () {

	$('.block').click(function () {
		if ($(this).find('i').hasClass('glyphicon-chevron-up')) {
			$(this).find('i').removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
			$('.OvertimeSetting, .OvertimeTitle').hide();
		} else {
			$(this).find('i').removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
			$('.OvertimeSetting, .OvertimeTitle').show();
		}
	});

	$("[selectAll]").change(function () {
		$($(this).attr('selectAll')).prop("checked", $(this).prop("checked"));
	});

	var addRows = function (responses) {
		$.each(responses, function (i, response) {
			//debugger;
			var $res = $(response).appendTo($('.OvertimeSummary table'));
			$res.find(".startDateTime,.endDateTime input").datetimepicker();
			$res.find(".employeeName select").change(function () {
				//console.log($(this));
				var $this = $(this);
				$this.closest('tr').find('.employeeID input').val($this.val());
				//debugger;
				var nationalType = $this.find("option:selected").attr('nationaltype');
				//var nationalType = $(this).children(":selected").attr('nationaltype');
				$this.closest('tr').find('.nationalType').val(nationalType);
				//先清空部門資料, 等待 response取回
				$this.closest('tr').find('.departmentID').val('');
				$this.closest('tr').find('.departmentName').val('');

				$.ajax({
					type: "post",
					url: "../../../Service/Sign/Forms/Overtime.asmx/GetDepartmentData",
					data: { "employeeID": $this.closest('tr').find('.employeeID input').val() },
					success: function (responses) {
						$this.closest('tr').find('.departmentID').val(responses['DepartmentID']);
						$this.closest('tr').find('.departmentName').val(responses['DepartmentName']);
					},
					error: function (e) {
						alert('新增多列失敗!' + e.responseJSON.ErrorMessage);
						console.log(e.responseJSON);
					}
				});
			})
		});
	};

	var getDefaultData = function () {
		return {
			"ApplyID_FK": $('#ApplyID_FK').val(),
			"DepartmentID_FK": $('#DepartmentID_FK').val(),
			"DefaultDeptID": $('#DefaultDeptName option:selected').val(),
			"DefaultDeptName": $('#DefaultDeptName option:selected').text(),
			"DefaultStartDateTime": $('#DefaultStartDateTime').val(),
			"DefaultSupportDeptID": $('#DefaultSupportDeptName option:selected').val(),
			"DefaultSupportDeptName": $('#DefaultSupportDeptName  option:selected').text(),
			"DefaultMealOrderKey": $('#DefaultMealOrderValue option:selected').val(),
			"DefaultMealOrderValue": $('#DefaultMealOrderValue option:selected').text(),
			"DefaultPayTypeKey": $('#DefaultPayTypeValue option:selected').val(),
			"DefaultPayTypeValue": $('#DefaultPayTypeValue option:selected').text(),
			"DefaultEndDateTime": $('#DefaultEndDateTime').val(),
			"DefaultNote": $('#DefaultNote').val(),
		};
	}

	$('.CreateDetailData').click(function () {
		var defaultData = getDefaultData();
		var isValid = true;
		$('.required input, .required select').each(function () {
			if (!this.value) {
				isValid = false;
				return false;
			};
		});
		if (!isValid) { alert('預設支援單位為必填!\n預設加班時間(起)為必填!\n預設加班時間(迄)為必填!'); return false; }


		$('.OvertimeSetting, .OvertimeTitle').hide();
		$('.OvertimeSummary, .save-footer').removeClass('hidden');
		$('.block').click();



		$.ajax({
			type: "post",
			url: "../../../Service/Sign/Forms/Overtime.asmx/CreateTableByDefaultValue",
			data: defaultData,
			success: function (responses) {
				addRows(responses);
			},
			beforeSend: function () {
				$('.create-footer .CreateDetailData').hide();
				$('.create-footer.CoverBtn').show();
				$('.data-loadding').show();
				$('.save-footer').hide();
				$('.AddRow').hide();
			},
			complete: function () {
				$('.create-footer .CreateDetailData').show();
				$('.create-footer .CoverBtn').hide();
				$('.data-loadding').hide();
				$('.save-footer').show();
				$('.AddRow').show();
			},
			error: function (e) {
				alert("產生明細資料失敗!" + e.responseJSON.ErrorMessage);
				console.log(e.responseJSON);
			}
		});
	});

	$('.AddRow').click(function () {
		var defaultData = getDefaultData();
		defaultData['IsAddRow'] = 'True';
		//debugger;
		var $NewRowEmpID = $('#NewRowEmpID');
		if ($NewRowEmpID.val()) { defaultData.NewRowEmpID = $NewRowEmpID.val(); }

		$.ajax({
			type: "post",
			url: "../../../Service/Sign/Forms/Overtime.asmx/CreateTableByDefaultValue",
			data: defaultData,
			success: function (responses) {
				addRows(responses);
			},
			error: function (e) {
				alert(e.responseJSON.ErrorMessage);
				console.log(e.responseJSON);
			}
		});
	});

	$('.SaveData').click(function () {
		var applyID = $('#ApplyID_FK').val();
		var applyDeptID = $('#ApplyDeptID_FK').val();
		var signDocID = $('#SignDocID').val();
		var formSeries = $('#FormSeries').val();
		var request = [];

		var isValid = true;
		var verifyTargets = [
			{ 'required': 'employeeID' },
			{ 'required': 'startDateTime' },
			{ 'required': 'endDateTime' },
			{ 'required': 'payTypeKey' },
			{ 'required': 'mealOrderKey' },
			{ 'required': 'note' },
			{ 'required': 'departmentID' }];

		var errorMsg = '';
		$.each($('.OvertimeSummary tr'), function (i, $tr) {
			var $isCheck = $(this).find('.check input').prop('checked');
			if ($isCheck) {
				var data = {};
				data['sn'] = $(this).find('.sn').val();
				data['applyID'] = applyID;
				data['applyDeptID'] = applyDeptID;
				data['employeeID'] = $(this).find('.employeeName option:selected').val();
				data['employeeName'] = $(this).find('.employeeName option:selected').text().replace(data['employeeID'], '').trim();
				data['departmentID'] = $(this).find('.departmentID input').val();
				data['startDateTime'] = $(this).find('.startDateTime input').val();
				data['endDateTime'] = $(this).find('.endDateTime input').val();
				data['supportDeptID_FK'] = $(this).find('.supportDept option:selected').val();
				data['supportDeptName'] = $(this).find('.supportDept option:selected').text();
				data['note'] = $(this).find('.note input').val();
				data['payTypeKey'] = $(this).find('.payType option:selected').val();
				data['payTypeValue'] = $(this).find('.payType option:selected').text();
				data['mealOrderKey'] = $(this).find('.mealOrder option:selected').val();
				data['mealOrderValue'] = $(this).find('.mealOrder option:selected').text();
				data['nationalType'] = $(this).find('.nationalType').val();
				data['departmentID'] = $(this).find('.departmentID').val();
				data['departmentName'] = $(this).find('.departmentName').val();

				//required field : employeeID endDateTime mealOrderKey payTypeKey startDateTime note
				$.each(verifyTargets, function (j, target) {
					if (data[target.required] == '') {
						errorMsg += '第' + i + '列欄位 : ' + target.required + '為必填欄位!\n';
						isValid = false;
					}
				});
				if (data['startDateTime'] >= data['endDateTime']) {
					debugger;
					errorMsg += '第' + i + '列 : 加班時間(起)必須小於加班時間(迄)!\n';
					isValid = false;
				}
				request.push(data);
			}
		});

		//判斷是否勾選資料
		if (!request[0]) { alert('請勾選要儲存的資料1213!'); return false; }
		//判斷是否為相同單位
		var firstSupDeptID = request[0].supportDeptID_FK;
		var firstSupDeptName = request[0].supportDeptName;
		var employeeIDContent = [];
		$.each($(request), function (index, data) {
			//debugger;
			if (data['supportDeptID_FK'] != firstSupDeptID) {
				errorMsg += '同張簽核支援單位必須相同!';
				isValid = false;
			}
			employeeIDContent.push(data['employeeID']);
		});

		//判斷是否有重複資料
		if (employeeIDContent.length != $.unique(employeeIDContent).length) {
			errorMsg += '儲存的資料表中不得有重複的人員資料!';
			isValid = false;
		}

		if (!isValid) {
			alert(errorMsg);
			return false;
		}

		request.push({ "applyID": applyID }, { "currentSignLevelDeptID": firstSupDeptID }, { "currentSignLevelDeptName": firstSupDeptName }, { "signDocID": signDocID }, { "formSeries": formSeries });
		//debugger;
		$.ajax({
			type: "post",
			//contentType: "application/json",
			traditional: true,
			url: "../../../Service/Sign/Forms/Overtime.asmx/SaveData",
			data: JSON.stringify(request),
			success: function (responses) {
				if (responses['SignDocID']) {
					alert('存檔成功，請確認明細!');
					//debugger;
					$('.OvertimeSummary').attr("href", "/Area/Sign/Forms/OvertimeSummary.aspx?signDocID=" + responses['SignDocID']);
					$('.isClick').val('true');
					$('.OvertimeSummary').click();
					$('.isClick').val('false');
				} else {
					alert(responses['ErrorMessage']);
				}
			},
			error: function (e) {
				alert('存檔失敗!' + e.responseJSON.ErrorMessage);
				console.log(e.responseJSON);
			},
			beforeSend: function () {
				$('.save-footer .SaveData').hide();
				$('.save-footer .CoverBtn').show();
			},
			complete: function () {
				$('.save-footer .SaveData').show();
				$('.save-footer .CoverBtn').hide();
			},

		});
	});

	$('.ApplyForm').click(function () {
		var request = [];
		$.each($('#OvertimeSummaryGridView tr'), function (i, $tr) {
			var data = {};
			if ($(this).find('th').length > 0) { return; }
			data['sn'] = $(this).find('.sn').val();
			data['signDocID'] = $(this).find('.signDocID').text();
			data['employeeID'] = $(this).find('.employeeID').text();
			data['employeeName'] = $(this).find('.employeeName').text();
			data['nationalType'] = $(this).find('.nationalType').text();
			data['departmentID'] = $(this).find('.departmentID').val();
			data['departmentName'] = $(this).find('.departmentName').text();
			data['startDateTime'] = $(this).find('.startDateTime').text();
			data['endDateTime'] = $(this).find('.endDateTime').text();
			data['supportDeptID_FK'] = $(this).find('.supportDeptID').val();
			data['supportDeptName'] = $(this).find('.supportDeptName').text();
			data['payTypeKey'] = $(this).find('.payTypeKey').val();
			data['payTypeValue'] = $(this).find('.payTypeValue').text();
			data['mealOrderKey'] = $(this).find('.mealOrderKey').val();
			data['mealOrderValue'] = $(this).find('.mealOrderValue').text();
			data['note'] = $(this).find('.note').text();
			request.push(data);
		});

		var firstSupDeptID = request[0].supportDeptID_FK;
		var signDocID = request[0].signDocID;
		var formSeries = $("#FormSeries").val();
		request.push({ "currentSignLevelDeptID": firstSupDeptID }, { "signDocID": signDocID }, { "formSeries": formSeries });

		$.ajax({
			type: "post",
			url: "../../../Service/Sign/Forms/Overtime.asmx/ApplyForm",
			data: JSON.stringify(request),
			success: function (responses) {
				alert(responses['Message']);
				window.parent.location.href = "../WorkflowQueryList.aspx?orderField=SendDate&descending=True";
			},
			error: function (e) {
			    alert('存檔失敗!' + e.responseJSON.ErrorMessage);
				console.log(e.responseJSON);
			},
			beforeSend: function () {
				$('.modal-footerlest .ApplyForm').hide();
				$('.modal-footerlest .Coverbtn').show();
			},
			complete: function () {
				$('.modal-footerlest .ApplyForm').show();
				$('.modal-footerlest .Coverbtn').hide();
			},
		});
	});

	if ($('#SignDocID').val()) {
		$('.OvertimeSetting, .OvertimeTitle').hide();
		$('.block').find('i').removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
		$('.OvertimeSummary').attr("href", "/Area/Sign/Forms/OvertimeSummary.aspx?signDocID=" + $('#SignDocID').val());
		//編輯草稿 /駁回
		$.ajax({
			type: "post",
			url: "../../../Service/Sign/Forms/Overtime.asmx/QueryOvertimeData",
			data: { "SignDocID": $('#SignDocID').val() },
			success: function (responses) {
				addRows(responses);
				$('.OvertimeSummary, .save-footer').removeClass('hidden');
			},
			beforeSend: function () {
				$('.data-loadding').show();
			},
			complete: function () {
				$('.data-loadding').hide();
			}
		});
	}
});