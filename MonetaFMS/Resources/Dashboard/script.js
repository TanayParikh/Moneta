	function resizeCanvas() {
		var width = window.innerWidth - 60;
		var height = window.innerHeight - 80;
		
		var canvs = document.getElementById("invoices-chart-area");
		canvs.width = width;
		canvs.height = height / 2;
		
		
		canvs = document.getElementById("expenses-chart-area");
		canvs.width = width / 2;
		canvs.height = height / 2;
		
		
		canvs = document.getElementById("clients-chart-area");
		canvs.width = width / 2;
		canvs.height = height / 2;
	}

	resizeCanvas();

	Chart.plugins.register({
		afterDraw: function(chart) {
		if (chart.data.datasets.length === 0) {
			// No data is present
		  var ctx = chart.chart.ctx;
		  var width = chart.chart.width;
		  var height = chart.chart.height
		  chart.clear();
		  
		  ctx.save();
		  ctx.textAlign = 'center';
		  ctx.textBaseline = 'middle';
		  ctx.font = "16px normal 'Helvetica Nueue'";
		  ctx.fillText(chart.options.emptyContent, width / 2, height / 2);
		  ctx.restore();
		}
	  }
	});

	var pieDataColors =
	[
		chartColors.red,
		chartColors.orange,
		chartColors.yellow,
		chartColors.green,
		chartColors.blue,
	];

	var pieTooltip =
	{
		enabled: true,
		mode: 'single',
		callbacks: {
			title: (tooltipItems, data) => {
				return data.labels[tooltipItems[0].index]
			},
			label: (tooltipItem, data) => { 
				//get the concerned dataset
				var dataset = data.datasets[tooltipItem.datasetIndex];
				
				//calculate the total of this data set
				var total = dataset.data.reduce(function(previousValue, currentValue, currentIndex, array) {
				return previousValue + currentValue;
				});
				
				//get the current items value
				var currentValue = dataset.data[tooltipItem.index];
				//calculate the precentage based on the total and current item, also this does a rough rounding to give a whole number
				var precentage = Math.floor(((currentValue/total) * 100)+0.5);

				return ' $' + currentValue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' (' + precentage + "%)";
			}
		}
	};

	function setupCharts()
	{
		var ctx = document.getElementById("clients-chart-area").getContext("2d");
		window.myPie = new Chart(ctx, 
			{
				type: 'doughnut',
				options: {
					responsive: true,
					legend: {
						display: true,
						position: 'right'
					},
					tooltips: pieTooltip,
					emptyContent: 'Your top clients by revenue will be shown here',
				}
			}
		);
		
		
		var ctx = document.getElementById("expenses-chart-area").getContext("2d");
		window.myExpensesDonut = new Chart(ctx, 
			{
				type: 'doughnut',
				options: {
					responsive: true,
					legend: {
						display: true,
						position: 'right'
					},
					tooltips: pieTooltip,
					emptyContent: 'Your top expense categories will be shown here',
				}
			}
		);
		
		var ctx = document.getElementById('invoices-chart-area').getContext('2d');
		window.revenueExpensesLineChart = new Chart(ctx, 
			{
			  type: 'line',
			  options: {
				tooltips: pieTooltip,
				emptyContent: 'Get started by adding new invoices or expenses!',
				scales: {
					yAxes: [{
						ticks: {
							// Include a dollar sign in the ticks
							callback: function(value, index, values) {
								return '$' + value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
							}
						}
					}]
				}
			  }
			}
		);
	}

	setupCharts();
			
	function setupTopClients(data)
	{
		window.myPie.config.data = JSON.parse(data);
		window.myPie.update();
		
		return "Success";
	}

	function setupTopExpenseCategories(data)
	{
		window.myExpensesDonut.config.data = JSON.parse(data);
		window.myExpensesDonut.update();
		
		return "Success";
	}

	function setupRevenueExpense(data)
	{
		window.revenueExpensesLineChart.config.data = JSON.parse(data);
		window.revenueExpensesLineChart.update();
		
		return "Success";
	}