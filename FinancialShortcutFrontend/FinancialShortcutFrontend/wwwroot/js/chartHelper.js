window.chartHelper = {
    charts: {},

    // Helper function to sanitize dataset properties
    sanitizeDataset: function(dataset) {
        const sanitized = { ...dataset };

        // Remove null/undefined values and replace with undefined
        // This prevents Chart.js from complaining about null values
        Object.keys(sanitized).forEach(key => {
            if (sanitized[key] === null) {
                delete sanitized[key];
            }
        });

        return sanitized;
    },

    createChart: function (canvasId, type, labels, datasets, options) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) {
            console.error('Canvas element not found:', canvasId);
            return false;
        }

        // Destroy existing chart if it exists
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        // Sanitize datasets
        const sanitizedDatasets = datasets.map(ds => this.sanitizeDataset(ds));

        // Default options if not provided
        const chartOptions = options || {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Financial Chart'
                }
            }
        };

        try {
            // Create new chart
            this.charts[canvasId] = new Chart(ctx, {
                type: type,
                data: {
                    labels: labels,
                    datasets: sanitizedDatasets
                },
                options: chartOptions
            });

            return true;
        } catch (error) {
            console.error('Error creating chart:', error);
            return false;
        }
    },

    updateChart: function (canvasId, labels, datasets) {
        if (this.charts[canvasId]) {
            try {
                // Sanitize datasets
                const sanitizedDatasets = datasets.map(ds => this.sanitizeDataset(ds));

                this.charts[canvasId].data.labels = labels;
                this.charts[canvasId].data.datasets = sanitizedDatasets;
                this.charts[canvasId].update();
                return true;
            } catch (error) {
                console.error('Error updating chart:', error);
                return false;
            }
        }
        return false;
    },

    destroyChart: function (canvasId) {
        if (this.charts[canvasId]) {
            try {
                this.charts[canvasId].destroy();
                delete this.charts[canvasId];
                return true;
            } catch (error) {
                console.error('Error destroying chart:', error);
                return false;
            }
        }
        return false;
    }
};
