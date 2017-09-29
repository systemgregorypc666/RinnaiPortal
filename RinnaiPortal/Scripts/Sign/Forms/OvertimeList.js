$(function () {
	var overworkHRPrinciple = 8;

	var YMD = $('.StartDateTime span')[0];
	var YM = YMD.textContent.match(/\d{4}-\d{2}/);
	var OverWorkHoursTHText = YM + "月加班總工時";
	$('#OverWorkHours').text(OverWorkHoursTHText);

	$('.OverWorkHours').each(function () {
		if ($(this).text() >= overworkHRPrinciple) {
			var $this = $(this);
			var tmptxt = $this.text();
			$this.text('本月總加班工時 : ' + tmptxt + ",已達法定" + overworkHRPrinciple + "小時標準!");
			$this.attr('style', 'color: red; font-weight: bolder;');
			
		}
	})
});