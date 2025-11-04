@RestControllerAdvice
public class ErrorControllerAdvice {

    @ExceptionHandler(Exception.class)
    public ResponseEntity&lt;String&gt; handleGenericException(Exception e) {
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body("An unexpected error occurred: " + e.getMessage());
    }
}